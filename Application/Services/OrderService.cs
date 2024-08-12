using Application.Interfaces;
using Application.SignalR;
using Ardalis.GuardClauses;
using Core.DTOS;
using Core.DTOS.Basket;
using Core.Entities;
using Core.Entities.Delivery;
using Core.Entities.Order;
using Core.Entities.Prescription_Lenses;
using Core.Products;
using Infrastructure.Identity;
using Infrastructure.interfaces;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class OrderService : IOrderInterface
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICacheService _basket;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DeliveryCostPlanInterface _deliveryCostPlan;
        private readonly IPrescriptionService _prescriptionService;
        private readonly ICacheService _basketRepository;

        public OrderService(ApplicationDbContext applicationDb,
            ICacheService redisService,
            UserManager<ApplicationUser> usermanger,
            DeliveryCostPlanInterface deliveryCostPlan,
            IPrescriptionService prescriptionService,
            ICacheService basketRepository
            )
        {
            this._dbContext = applicationDb;
            this._basket = redisService;
            _userManager = usermanger;
            _deliveryCostPlan = deliveryCostPlan;
            _prescriptionService = prescriptionService;
            _basketRepository = basketRepository;
        }

        public async Task<Guid?> CreateOrderBasketAsync(
            Guid userId,
            string firstName,
            string lastName,
            Guid basketId,
            bool? isAndroidDevice,
            Guid? cityId,
            string? street, string?
            newPhoneNumber, Guid? existPhoneNumberId,
            Guid? addressId)
        {

            var basket = _basket.GetData<CustomerBasket>(basketId.ToString());

            if (basket == null)
                return null;

            var items = await GetOrderItemsAsync(basket.Items);


            Address address = await 
                GetAddressAsync(firstName,lastName,userId, cityId, street, addressId);


            var phoneNumber = await GetPhoneNumberAsync(userId, newPhoneNumber, existPhoneNumberId);


            var deliveryPrice = await CalculateDeliveryPriceAsync(address.CityId);


            var subTotal = CalculateSubTotal(items, deliveryPrice);


            var order = new Order(items, address, subTotal, userId,phoneNumber);

            order.EstimatedDeliveryTime = CalculateEstimatedDeliveryTime(order); // Set estimated delivery time


            await SaveOrderAsync(order);



            _basketRepository.RemoveData(basketId.ToString());

            
            return order.Id;
        }

        public async Task<Guid?> CreateSingleProductOrderAsync(
              Guid userId,
              string firstName,
              string lastName,
              Guid productId,
              string colorName,
              bool? isAndroidDevice,
              Guid? cityId,
              string? street,
              string? newPhoneNumber,
              Guid? existPhoneNumberId,
              Guid? addressId,
              Guid? userPrescrptionId)
        {
            var product = await _dbContext.Products
                .Include(p => p.ProductColors)
                .Include(p => p.ProductGenderTypes)
                .Include(p => p.PicturesUrl)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
            {
                throw new NotFoundException("Product not found", nameof(Product));
            }

            Address address = await GetAddressAsync(
                firstName,lastName,
                userId, cityId, street, addressId);

            var phoneNumber = await GetPhoneNumberAsync(userId, newPhoneNumber, existPhoneNumberId);

            decimal prescriptionPrice = 0;

            if (product is Glass && userPrescrptionId is not null)
            {
                var userPrescription = await _prescriptionService.GetUserPrescription(userId);

                var priceRange = await _dbContext.PriceRanges
                    .FirstOrDefaultAsync(pr =>
                      pr.ApplicationManagerID == product.ManagerId &&
                      pr.SphereMin <= userPrescription.DistanceSphereRight &&
                      pr.SphereMax >= userPrescription.DistanceSphereRight &&
                      pr.CylinderMin <= userPrescription.DistanceCylinderRight &&
                      pr.CylinderMax >= userPrescription.DistanceCylinderRight &&
                      pr.CoatingTypeID == userPrescription.CoatingTypeId &&
                      pr.VendorCountryID == userPrescription.CountryTypeId);

                if (priceRange != null)
                {
                    prescriptionPrice = priceRange.Price;
                }
                else
                {
                    throw new Exception("Price range not found for the selected prescription"); // Or handle differently
                }
            }

            var deliveryPrice = await 
                CalculateDeliveryPriceAsync(address.CityId);

            var productImage = product.PicturesUrl?.FirstOrDefault(p => p.IsDefault)?.Name;

            var itemOrdered = new ProductItemOrdered
            {
                ProductItemId = product.Id,
                ProductName = product.ProductName,
                ProductImage = productImage
            };

            var orderItem = new OrderItem(
                itemOrdered, 1, product.Price,
                colorName, product.ManagerId, userPrescrptionId);

            var order = new Order(new List<OrderItem> { orderItem },
                address, product.Price + deliveryPrice + prescriptionPrice,
                userId, phoneNumber);

            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try { 
                  await SaveOrderAsync(order);
                }catch(Exception ex)
                {

                }

                product.AvailableQuantity--;
                
                product.MostPopular++;

                order.EstimatedDeliveryTime = CalculateEstimatedDeliveryTime(order); // Set estimated delivery time


                await _dbContext.SaveChangesAsync();
                
                transaction.Commit();
            }

            return order.Id;
        }



        private async Task<Address> GetAddressAsync(
            string firstName , string lastName,Guid userId, 
            Guid? cityId, string street, Guid? addressId)
        {
            if (addressId != null)
            {
                return await _dbContext.Address.FirstOrDefaultAsync(a => a.Id == addressId);
            }

            if(cityId == null) {
                throw new Exception(" the cityId cannot be null when the addressId is null");
            }

            // Check if the address already exists
            var existingAddress = await _dbContext.Address
                .FirstOrDefaultAsync(a => a.FirstName == firstName &&
                                          a.LastName == lastName &&
                                          a.UserId == userId &&
                                          a.CityId == cityId &&
                                          a.Street == street);
            if (existingAddress != null)
            {
                return existingAddress;
            }

            // If the address doesn't exist, create a new one
            var newAddress = new Address
            {
                CityId = cityId.Value,
                Street = street,
                UserId = userId,
                FirstName = firstName,
                LastName = lastName,
            };

            _dbContext.Address.Add(newAddress);
            
            await _dbContext.SaveChangesAsync();

            return newAddress;
        }

        private async Task SaveOrderAsync(Order order)
        {
            _dbContext.Entry(order.ShipToAddress).State = EntityState.Unchanged;

            await _dbContext.Orders.AddAsync(order);

            await _dbContext.SaveChangesAsync();
        }

        private async Task<List<OrderItem>> GetOrderItemsAsync(
            List<BasketItem> basketItems)
        {
            var items = new List<OrderItem>();

            foreach (var item in basketItems)
            {
                var productItem = await _dbContext
                    .Products
                    .Include(p => p.ProductColors)
                    .Include(p => p.PicturesUrl) 
                    .FirstOrDefaultAsync(p => p.Id == item.Id);

                UserPrescriptionDtoResponse? prescriptionPrice = null;

                if (productItem is Glass && item.PrescriptionId != null)
                {
                    var prescription = await _dbContext.UserPrescriptions
                        .FirstOrDefaultAsync(p => p.Id == item.PrescriptionId);

                    if (prescription != null)
                    {
                        var priceRange = await _dbContext.PriceRanges.FirstOrDefaultAsync(pr =>
                            pr.ApplicationManagerID == productItem.ManagerId &&
                            pr.SphereMin <= prescription.DistanceSphereRight &&
                            pr.SphereMax >= prescription.DistanceSphereRight &&
                            pr.CylinderMin <= prescription.DistanceCylinderRight &&
                            pr.CylinderMax >= prescription.DistanceCylinderRight &&
                            pr.CoatingTypeID == prescription.CoatingTypeID &&
                            pr.VendorCountryID == prescription.VendorCountryID);

                        if (priceRange != null)
                        {
                            prescriptionPrice = new UserPrescriptionDtoResponse
                            {
                                Price = priceRange.Price
                            };
                        }
                        else
                        {
                            throw new Exception("Price range not found for the selected prescription");
                        }
                    }
                    else
                    {
                        throw new Exception("Prescription not found");
                    }
                }

                var itemOrdered = new ProductItemOrdered
                {
                    ProductItemId = productItem.Id,
                    ProductName = productItem.ProductName,
                    ProductImage = productItem.PicturesUrl.FirstOrDefault(p => p.IsDefault)?.ToString()
                };

                var orderItem = new OrderItem(
                    itemOrdered,
                    item.Quantity,
                    productItem.Price + prescriptionPrice?.Price?? 0, // Include prescription price in the item's price
                    item.Color,
                    productItem.ManagerId,
                    item.PrescriptionId
                );

                items.Add(orderItem);
            }

            return items;
        }


        private async Task<decimal> CalculateDeliveryPriceAsync(
            Guid cityId)
        {

            var effectiveCostPlan = 
                await _deliveryCostPlan.GetEffectiveCostPlan();

            var cityPlan = effectiveCostPlan
                .DeliveryCostDetails
                             .FirstOrDefault(x => x.CityId == cityId);

            if(cityPlan == null)
            {
                throw new Exception("No delivery added for this city");
            }

            return cityPlan.Price;
        }

        private async Task<string>
            GetPhoneNumberAsync(Guid userId, string? newPhoneNumber, Guid? existPhoneNumberId)
        {
            if (existPhoneNumberId != null)
            {
                var phone = await 
                    _dbContext.PhoneNumbers.FirstOrDefaultAsync(p => p.Id == existPhoneNumberId);
                return phone?.Phone;
            }

            
            var isPhoneNumberExists = 
                _dbContext.PhoneNumbers.Any(p => p.Phone == newPhoneNumber);

            if (!isPhoneNumberExists)
            {
                await _dbContext.PhoneNumbers.AddAsync(new PhoneNumber
                {
                    Phone = newPhoneNumber,
                    ApplicationUserId = userId
                });


            }

            return newPhoneNumber;
        }


        private decimal CalculateSubTotal(List<OrderItem> items, decimal deliveryPrice)
        {
            return items.Sum(i => i.Quantity * i.Price) + deliveryPrice;
        }

      
        public async Task<Order?> GetOrderById(Guid id)
        {
            return await _dbContext.Orders.Include(o => o.OrderItems)
                .Include(o => o.ShipToAddress).FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<List<Order>?> GetOrders(string baseUrl, Guid? userId)
        {
            var orders = await _dbContext.Orders
                .Where(o => userId == null || o.UserId == userId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.UserPrescription)
                    .ThenInclude(up => up.CoatingType)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.UserPrescription)
                    .ThenInclude(up => up.LensType)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.UserPrescription)
                    .ThenInclude(up => up.Country)
                .Include(o => o.ShipToAddress)
                .ToListAsync();

            // Modify each order item to include complete image URLs
            foreach (var order in orders)
            {
                foreach (var orderItem in order.OrderItems)
                {
                    // Assuming baseUrl is a parameter or property containing the base URL
                    orderItem.ItemOrdered.ProductImage = Path.Combine(
                        baseUrl,
                        "managersProductsPhotos",
                        orderItem.ManagerID.ToString(),
                        orderItem.ItemOrdered.ProductItemId.ToString(),
                        orderItem.ItemOrdered.ProductImage);
                }
            }

            return orders;
        }
        public async Task<List<Order>?> GetOrdersForManager(Guid managerId)
        {
            var managerOrders =
                 await _dbContext.Orders
                 .Include(o => o.OrderItems)
                 .Where(o => o.OrderItems.Any(oi => oi.ManagerID == managerId))
                 .ToListAsync();

            if (managerOrders is null)
                return null;

            return managerOrders;
        }

        public async Task<IReadOnlyList<Order>?> GetOrdersAsync()
        {
            var orders = await _dbContext.Orders.ToListAsync();

            return orders ?? null;
        }


        public async Task<bool> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus)
        {
            var order = await _dbContext
                .Orders.FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return false; 
            }

            if(!IsValidStatusTransition(order.Status, newStatus))  // validate the new status
            {
                return false;
            }

            order.Status = newStatus;


            await _dbContext.SaveChangesAsync();

            return true; 
        }

        private bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            switch (currentStatus)
            {
                case OrderStatus.Pending:
                    return newStatus == OrderStatus.Processing || newStatus == OrderStatus.Cancelled;

                case OrderStatus.Processing:
                    return newStatus == OrderStatus.Shipped || 
                        newStatus == OrderStatus.Cancelled ||
                        newStatus == OrderStatus.PaymentFailed;

                case OrderStatus.Shipped:
                    return newStatus == OrderStatus.Delivered || newStatus == OrderStatus.Returned;

                case OrderStatus.Delivered:
                    return false;

                case OrderStatus.Cancelled:
                    return false;

                case OrderStatus.Returned:
                    return newStatus == OrderStatus.Refunded;

                case OrderStatus.Refunded:
                    return false;

                case OrderStatus.PaymentFailed:
                    return newStatus == OrderStatus.Cancelled;

                default:
                    return false; // Unknown status, consider this an invalid transition.
            }
        }

        public DateTime CalculateEstimatedDeliveryTime(Order order)
        {
            int processingDays = DeliveryConstants.StandardProcessingDays;
            int shippingDays = DeliveryConstants.StandardShippingDays;
            DateTime currentDate = DateTime.Now;

            // Calculate processing end date
            DateTime estimatedProcessingEndDate = AddBusinessDays(currentDate, processingDays);

            // Calculate estimated delivery date
            DateTime estimatedDeliveryDate = AddBusinessDays(estimatedProcessingEndDate, shippingDays);

            return estimatedDeliveryDate;
        }


        private DateTime AddBusinessDays(DateTime startDate, int businessDays)
        {
            if (businessDays < 0)
                throw new ArgumentException("businessDays cannot be negative");

            DateTime currentDate = startDate;
            while (businessDays > 0)
            {
                currentDate = currentDate.AddDays(1);
                if (currentDate.DayOfWeek != DayOfWeek.Friday)
                {
                    businessDays--;
                }
            }
            return currentDate;
        }
        public static bool IsBusinessDay(DateTime date)
        {
            return date.DayOfWeek != DayOfWeek.Friday;
        }

    }
}
