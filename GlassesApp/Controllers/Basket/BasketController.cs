using Core.DTOS;
using Core.DTOS.Basket;
using Infrastructure.Identity;
using Infrastructure.interfaces;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Tensorflow;

namespace GlassesApp.Controllers.Basket
{

    public class BasketController : BaseApiController
    {
        private readonly ICacheService _basketRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;
        public BasketController(ICacheService basketRepository,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext db)
        {
            _basketRepository = basketRepository;
            _userManager = userManager;
            _db = db;
        }
        
        [HttpGet]
        [AllowAnonymous]
        public ActionResult<CustomerBasket> GetBasketById(
            string id)
        {
            if (!Guid.TryParse(id, out _))
            {
                // Return a bad request response with a custom error message
                return BadRequest("Invalid id format. Please provide a valid GUID.");
            }

            var basket = _basketRepository.GetData<CustomerBasket>(id);

            if (basket == null)
            {
                return new CustomerBasket(Guid.Parse(id));
                // return a empty bakset for the invalid id because not store any empty data to redis
                // it's not makesence to add a basket when the id is not valid
            }

            return Ok(basket);

        }

        [HttpPost]
        [AllowAnonymous]
        
        public async Task<ActionResult<CustomerBasket>> 
            UpdateBasket(
            CustomerBasket basket)
        {

            // Retrieve the existing basket
            var existingBasket = _basketRepository
                .GetData<CustomerBasket>(basket.Id.ToString());

            if (existingBasket == null)
            {
                // If the basket does not exist, create a new one
                existingBasket = new CustomerBasket(basket.Id)
                {
                    UserId = basket.UserId,
                    Items = basket.Items
                };
            }
            else
            {
                // If the basket exists, add the new items to the existing items
                foreach (var newItem in basket.Items)
                {
                    var existingItem = existingBasket.Items.FirstOrDefault(
                        item => item.Id == newItem.Id);
                    if (existingItem != null)
                    {
                        // If the item already exists, increase the quantity
                        existingItem.Quantity += newItem.Quantity;
                    }
                    else
                    {
                        // If the item does not exist, add it to the basket
                        existingBasket.Items.Add(newItem);
                    }
                }
            }

            // Save the updated basket
            var updateBasket = _basketRepository.SetData<CustomerBasket>(basket.Id.ToString(), existingBasket, DateTime.Now.AddDays(30));

            if (!updateBasket)
            {
                return BadRequest("Failed to update the basket.");
            }

            return Ok(existingBasket);
        }

        
        [HttpDelete("{id}")]
        [AllowAnonymous]

        public IActionResult DeleteBasket(string id)
        {
            var basket = _basketRepository.GetData<CustomerBasket>(id);

            if(basket == null)
                return NotFound("Basket not found");

            var prescriptionsIds = basket.Items.Select(x => x.PrescriptionId).ToList();

            foreach (var prescriptionId in prescriptionsIds)
            {
                if (prescriptionId != null)
                {
                    var prescriptionToRemove = _db.UserPrescriptions.FirstOrDefault(p => p.Id == prescriptionId);

                    // remove the prescription only if the user NOT Logged
                    if (prescriptionToRemove != null && prescriptionToRemove.UserID != null)
                    {
                        _db.UserPrescriptions.Remove(prescriptionToRemove);
                    }
                }
            }
            var result = _basketRepository.RemoveData(id);

            if (!result)
                return NotFound("the basket item not exist");
            //throw exception or return bad request or what ?

            return Ok(result);
        }


        [HttpDelete("{basketId}/item/{itemId}")]
        [AllowAnonymous]

        public IActionResult DeleteItemFromBasket(
            string basketId, string itemId)
        {
            // Note: when remove the item from the database or remove all the baskets , 
            // must remove all the prescirptions from the database(sql not the InMemeory)
            var basket = _basketRepository.GetData<CustomerBasket>(basketId);

            if (basket == null)
            {
                return NotFound("Basket not found");
            }

            var itemToRemove = basket.Items.Find(item => item.Id ==
            Guid.Parse(itemId));

            if (itemToRemove == null)
            {
                return NotFound("Item not found in the basket");
            }
              
            if(itemToRemove.PrescriptionId != null)
            {
                var prescriptionToRemove = _db
                    .UserPrescriptions
                    .FirstOrDefault(p => p.Id == itemToRemove.PrescriptionId);

                // remove the prescription only if the user NOT Logged
                if (prescriptionToRemove != null && 
                    prescriptionToRemove.UserID == null)
                {
                    _db.UserPrescriptions.Remove(prescriptionToRemove);
                }
            }


            basket.Items.Remove(itemToRemove);

            _basketRepository.SetData<CustomerBasket>(basketId,
                basket,
                DateTime.Now.AddDays(30));

            return Ok("Item removed from the basket");
        }


        [HttpPost("assign-user-basket")]
        [Authorize(Roles = "AppUser")]
        public async Task<IActionResult> 
            AssignUserToBasket([FromBody]
        BasketDTO basketDTO)
        {
            var user = await _userManager.FindByIdAsync(User.FindFirst("ID")!.Value);

            // Retrieve the user's basket, if any
            var userBasket = _basketRepository
                .GetBasketByUserId(user.Id.ToString());

            // Retrieve the existing basket by ID
            var existingBasket = _basketRepository.GetData
                <CustomerBasket>(basketDTO.BasketID);

            if (existingBasket == null)
            {
                return NotFound("Basket not found");
            }

            // Associate the user ID with the basket
            existingBasket.UserId = user.Id;

            // Update the user ID in prescriptions associated with items in the basket
            foreach (var item in existingBasket.Items)
            {
                // Check if the item has a prescription ID
                if (item.PrescriptionId != null)
                {
                    // Retrieve the prescription from the database
                    var prescriptionToUpdate = _db.
                        UserPrescriptions.FirstOrDefault
                        (p => p.Id == item.PrescriptionId);

                    // Update the user ID in the prescription
                    if (prescriptionToUpdate != null)
                    {
                        prescriptionToUpdate.UserID = user.Id;
                        // Save changes to the database
                        await _db.SaveChangesAsync();
                    }
                }
            }

      

            // if the user already has a basket, merge items
            // otherwise, assign the existing basket to the user

            if (userBasket != null)
            {
                foreach (var item in existingBasket.Items)
                {
                    var existingItem = userBasket.Items
                        .FirstOrDefault(i => i.Id == item.Id);

                    if (existingItem == null)
                    {
                        userBasket.Items.Add(item);
                    }
                    else
                    {
                        existingItem.Quantity += item.Quantity;
                    }
                }


                _basketRepository.SetData(userBasket.Id.ToString(), 
                    userBasket, DateTime.Now.AddDays(30));

                _basketRepository.RemoveData(basketDTO.BasketID);
            }
            else
            {
                _basketRepository.SetData(existingBasket.Id.ToString(), existingBasket, DateTime.Now.AddDays(30));
            }

            return Ok("Basket associated with user correctly");
        }

        [Authorize(Roles = "AppUser")]
        [HttpGet("get-user-basket")]
        public async Task<ActionResult<CustomerBasket>> 
            GetUserBasket()
        {
            var user = await _userManager
               .FindByIdAsync(User.FindFirst("ID")!.Value);

            return _basketRepository.GetBasketByUserId(user.Id.ToString());

        }


        [HttpPost("add-to-basket")]
        [AllowAnonymous]

        public IActionResult AddToBasket([FromBody] AddToBasketRequestDto request)
        {
            var existingBasket = 
                _basketRepository.GetData<CustomerBasket>
                (request.BasketId);

            if (existingBasket == null)
            {
                return BadRequest("Invalid basketId");
            }

            var existingItem = existingBasket.Items.FirstOrDefault(i => i.Id == request.NewItem.Id);

            if (existingItem == null)
            {
                // If the item doesn't exist in the basket, add it
                existingBasket.Items.Add(request.NewItem);
            }
            else
            {
                // If the item already exists, increase its quantity
                ++existingItem.Quantity;
            }

            _basketRepository.SetData<CustomerBasket>(request.BasketId, existingBasket, DateTime.Now.AddDays(30));

            return Ok("Item added to the basket successfully");
        }

        
        [HttpPut("{basketId}/decrease-item-quantity/{itemId}")]
        [AllowAnonymous]

        public IActionResult IncreaseOrDecreaseItemQuantity(
            IncreaseOrDecreaseItemQuantityDTO inCrease)
        {
            var basket = _basketRepository.GetData<CustomerBasket>(inCrease.BasketId);

            if (basket == null)
            {
                return NotFound("Basket not found");
            }

            var itemToDecrease = basket.Items
                .FirstOrDefault(item => item.Id == Guid.Parse(inCrease.ItemId));

            if (itemToDecrease == null)
            {
                return NotFound("Item not found in the basket");
            }

            if (itemToDecrease.Quantity <= 1)
            {
                // If the item quantity is already 1 or less, remove it from the basket
                basket.Items.Remove(itemToDecrease);
            }
            if(!inCrease.Increased)
            {
                itemToDecrease.Quantity--;
            }
            else
            {
                itemToDecrease.Quantity++;
            }

            _basketRepository.SetData<CustomerBasket>(inCrease.BasketId, basket, DateTime.Now.AddDays(30));

            return Ok("Item quantity decreased successfully");
        }

        [HttpGet("check-item-in-basket")]
        [AllowAnonymous]
        public ActionResult<bool> CheckItemInBasket(string basketId,
            string itemId)
        {
            var basket = _basketRepository.GetData<CustomerBasket>(basketId);

            if (basket == null)
            {
                basket = new CustomerBasket(Guid.Parse(basketId));
                _basketRepository.SetData(basketId, basket, DateTime.Now.AddDays(30));
                return Ok(false);
            }

            var itemExists = basket.Items.Any(item => item.Id == Guid.Parse(itemId));

            return Ok(itemExists);
        }

        [HttpPut]
        [AllowAnonymous]
        public IActionResult UpdateQuantities(
         UpdateQuantityDTO updateQuantityDTO)
        {
            if (!updateQuantityDTO.itemQuantityDTO.Any())
            {
                return BadRequest("Invalid update data");
            }
            var basket = _basketRepository.GetData<CustomerBasket>(
                updateQuantityDTO.BasketID);

            if (basket == null)
            {
                return NotFound("Basket not found");
            }


            foreach (var itemQuantity in updateQuantityDTO.itemQuantityDTO)
            {
                var basketItem = basket.Items.FirstOrDefault(item => item.Id == itemQuantity.ProductId);
                if (basketItem != null)
                {
                    basketItem.Quantity = itemQuantity.Quantity;
                }

                else
                {
                    return BadRequest($"Item ID {itemQuantity.ProductId} not found in the basket");
                }
            }


            // is the following line necessary?
            var updateBasket = _basketRepository.SetData<CustomerBasket>(
                basket.Id.ToString(), 
                basket, DateTime.Now.AddDays(30));


            if (!updateBasket)
            {
                return BadRequest("Failed to update the basket");
            }

            return Ok(new MessageResponse { Message = "Quantities updated successfully" });

        }

    }
}