using Core.DTOS;
using Core.DTOS.ProductDTOS;
using Core.Entities;
using Core.enums;
using Core.Products;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using Tensorflow;

namespace GlassesApp.Controllers
{
    [Authorize(Roles ="AppUser")]
    public class FavoritesController : BaseApiController
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;

        public FavoritesController(UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _db = context;
        }

        [HttpPost]
        [Authorize(Roles = "AppUser")]
        public async Task<IActionResult> AddToFavorites(
            ProductIdDto productId)
        {
            var user = await _userManager.FindByIdAsync(User.FindFirst("ID")?.Value);

            if (user == null)
                return Unauthorized();

            var product = await _db.Products
                .FirstOrDefaultAsync(p => p.Id == productId.ProductId);

            if (product == null)
                return NotFound("product not found");

            // Check if the product is already in favorites
            var existingFavorite = await _db.UserFavorites
                .FirstOrDefaultAsync(ufp => ufp.ApplicationUserId == user.Id 
                && ufp.ProductId == productId.ProductId);

            if (existingFavorite != null)
                return Conflict("Product already in favorites");

            // Add the product to favorites
            var favorite = new UserFavoriteProduct {
                ApplicationUserId = user.Id, 
                ProductId =
                productId.ProductId
            };

            _db.UserFavorites.Add(favorite);

            await _db.SaveChangesAsync();

            return Ok(new MessageResponse { Message = "Product added to favorites" });
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveFromFavorites
            (Guid productId)
        {
            var user = await _userManager.FindByIdAsync(User.FindFirst("ID")?.Value);
            if (user == null)
                return Unauthorized(); // User not found

            // Check if the product is in favorites
            var existingFavorite = await _db.UserFavorites
                .FirstOrDefaultAsync(ufp => ufp.ApplicationUserId == user.Id && ufp.ProductId == productId);

            if (existingFavorite == null)
                return NotFound("Product not found in favorites");

            // Remove the product from favorites
            _db.UserFavorites.Remove(existingFavorite);

            await _db.SaveChangesAsync();

            return Ok("Product removed from favorites");
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductDTO>>> GetFavoriteProducts()
        {
            var user = await _userManager.FindByIdAsync(User.FindFirst("ID")?.Value);

            if (user == null)
                return NotFound("User does not exist");

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

            var userFavorites = await _db.UserFavorites
                .Where(x => x.ApplicationUserId == user.Id)
                .ToListAsync();

            if (userFavorites == null)
            {
                return StatusCode(500);
            }

            if (userFavorites.Count == 0)
                return Ok(new List<ProductDTO>());

            var favoriteProductIds = userFavorites.Select(f => f.ProductId).ToList();

            var favoriteProducts = await _db.Products
                .Include(p => p.PicturesUrl)
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.FavoriteProducts)
                .Include(p => p.ProductColors)
                    .ThenInclude(p => p.Color)
                .Include(p => p.ProductGenderTypes)
                    .ThenInclude(pgt => pgt.GenderType)
                .Where(p => favoriteProductIds.Contains(p.Id))
                .ToListAsync();

            var productDTOs = new List<ProductDTO>();

            foreach (var product in favoriteProducts)
            {
                var imagesURLs = product.PicturesUrl
                    .GroupBy(x => x.ColorId)
                    .Select(g => new ImageDTOResponse
                    {
     
                        ColorId = g.Key,
                        PictureDTO = g.Select(p => new PictureDTO
                        {
                            ImageId = p.Id,
                            IsDefault = p.IsDefault,
                            ImageUrl = Path.Combine(
                                baseUrl, "managersProductsPhotos",
                                product.ManagerId.ToString(),
                                product.Id.ToString(),
                                p.Name)
                        }).ToList()
                    }).ToList();

                var colorsNames = product.ProductColors.Select(pc => pc.Color.ColorName).ToList();
                
                int productType = 0;
                
                if (product is Accessory)
                      productType = 1;
                else if(product is Lense)
                    productType = 2;

                var commonDto = new ProductDTO
                {
                    ProductType = productType,
                    Id = product.Id,
                    ProductName = product.ProductName,
                    Price = product.Price,
                    Description = product.Description,
                    Brand = product.Brand.BrandName,
                    Category = product.Category.CategoryName,
                    DefaultPictureUrl = imagesURLs,
                    ColorsNames = colorsNames,
                    MostPopular = product.MostPopular,
                    AvailableQuantity = product.AvailableQuantity,
                    IsFavorite = product.FavoriteProducts.Any(x => x.ProductId == product.Id && x.ApplicationUserId == user.Id)
                };

                productDTOs.Add(commonDto);
            }

            return Ok(productDTOs);
        }


        private async Task<List<string>> GetGenderNames(List<Guid> genderIds)
        {
            var Genders = new List<string>();

            foreach (var genderId in genderIds)
            {
                var gender = await _db.GenderTypes.FindAsync(genderId);
                if (gender != null)
                {
                    Genders.Add(gender.GenderName);
                }
            }

            return Genders;
        }



    }

}
