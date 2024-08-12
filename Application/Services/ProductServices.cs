using API.Errors;
using Application.Interfaces;
using Application.pagination;
using AutoMapper;
using Core.DTOS;
using Core.DTOS.filteredDTO;
using Core.DTOS.ProductDTOS;
using Core.DTOS.ProductDTOS.UpdateDTOS;
using Core.Entities;
using Core.enums;
using Core.Products;
using Infrastructure.Identity;
using Infrastructure.Migrations;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ProductServices : IProductService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public ProductServices(ApplicationDbContext dbContext, IMapper mapper,
            UserManager<ApplicationUser> userManager, IWebHostEnvironment webHost)
        {
            _env = webHost;
            _dbContext = dbContext;
            this._mapper = mapper;
            this._userManager = userManager;
        }

        public async Task<bool> UpdateGlassImages(
            GlassUpdateImageDTO product, string baseUrl)
        {
            var glass = await _dbContext.Glasses
                .Include(x => x.PicturesUrl)
                .FirstOrDefaultAsync(x => x.Id == product.Id);

            if (glass == null)
                return false;

            // Clear existing images
            glass.PicturesUrl.Clear();

            var folderPath = Path.Combine(_env.WebRootPath, "managersProductsPhotos", glass.ManagerId.ToString(), glass.Id.ToString());

            if (Directory.Exists(folderPath))
            {
                Directory.Delete(folderPath, true);
            }

            Directory.CreateDirectory(folderPath);

            List<Image> images = new List<Image>();

            foreach (var image in product.PicturesUrl)
            {
                Guid imageUnique = Guid.NewGuid();
                var photoPath = Path.Combine(folderPath, $"{imageUnique + "_" + image.image.FileName}");

                using (var memoryStream = new MemoryStream())
                {
                    await image.image.CopyToAsync(memoryStream);
                    await System.IO.File.WriteAllBytesAsync(photoPath, memoryStream.ToArray());
                }

                var img = new Image
                {
                    Name = $"{imageUnique + "_" + image.image.FileName}",
                    ProductId = glass.Id,
                    ColorId = image.ColorId,
                    IsDefault = image.IsDefault
                };

                images.Add(img);
            }

            // Update the glass with the new images
            glass.PicturesUrl = images;

            await _dbContext.SaveChangesAsync();

            return true;
        }


        public async Task<PaginatedList<AccessoryDTOResponse>> GetManagerAccessoriesAsync(
     Guid managerId, AccessoryFilterDTO accessoryFilterDTO, string baseUrl,
     int pageIndex = 1, int pageSize = 10)
        {
            var filteredAccessories = await _dbContext.Accessories
                .Include(a => a.Brand)
                .Include(a => a.Category)
                .Include(a => a.PicturesUrl)
                .Include(a => a.Reviews)
                .Include(a => a.FavoriteProducts)
                .Include(a => a.ProductColors)
                    .ThenInclude(c => c.Color)
                .Include(a => a.ProductGenderTypes)
                    .ThenInclude(g => g.GenderType)
                .Where(a => a.ManagerId == managerId) // Filter by managerId
                .Where(a => (!accessoryFilterDTO.MinPrice.HasValue && !accessoryFilterDTO.MaxPrice.HasValue) ||
                            (a.Price >= (accessoryFilterDTO.MinPrice ?? 0) &&
                            (!accessoryFilterDTO.MaxPrice.HasValue || a.Price <= accessoryFilterDTO.MaxPrice.Value)))
                .Where(a => accessoryFilterDTO.GendersId == null || a.ProductGenderTypes.Any(p => accessoryFilterDTO.GendersId.Contains(p.GenderTypeId)))
                .Where(a => (accessoryFilterDTO.CategoryIds == null || accessoryFilterDTO.CategoryIds.Contains(a.CategoryId)))
                .Where(a => (accessoryFilterDTO.BrandIds == null || accessoryFilterDTO.BrandIds.Contains(a.BrandId)))
                .Select(a => new AccessoryDTOResponse
                {
                    Genders = a.ProductGenderTypes.Select(pgt => pgt.GenderType.GenderName).ToList(),
                    PictureUrl = a.PicturesUrl
                        .GroupBy(x => x.ColorId)
                        .Select(g => new ImageDTOResponse
                        {
                            ColorId = g.Key,
                            PictureDTO = g.Select(p => new PictureDTO
                            {
                                IsDefault = p.IsDefault,
                                ImageUrl = Path.Combine(baseUrl, "managersProductsPhotos", a.ManagerId.ToString(), a.Id.ToString(), p.Name)
                            }).ToList()
                        }).ToList(),
                    ProductName = a.ProductName,
                    Id = a.Id,
                    Price = a.Price,
                    Description = a.Description,
                    Brand = a.Brand.BrandName,
                    Category = a.Category.CategoryName,
                    ColorsNames = a.ProductColors.Select(pc => pc.Color.ColorName).ToList(),
                    AvailableQuantity = a.AvailableQuantity,
                    MostPopular = a.MostPopular,
                    IsFavorite = false,
                })
                .PaginatedListAsync(pageIndex, pageSize);

            return filteredAccessories;
        }

        public async Task<PaginatedList<GlassDTOResponse>> GetManagerGlassesAsync(
      Guid managerId, GlassFilterDTO glassFilterDTO, string baseUrl,
      int pageIndex = 1, int pageSize = 10)
        {
            var filteredGlasses = await _dbContext.Glasses
                .Include(a => a.Brand)
                .Include(a => a.Category)
                .Include(a => a.ProductGenderTypes)
                    .ThenInclude(pg => pg.GenderType)
                .Include(a => a.FrameType)
                .Include(a => a.Shape)
                .Include(a => a.ProductColors)
                    .ThenInclude(pg => pg.Color)
                .Include(a => a.PicturesUrl)
                .Include(a => a.FavoriteProducts)
                .Where(a => a.ManagerId == managerId) // Filter by managerId
                .Select(a => new GlassDTOResponse
                {
                    Genders = a.ProductGenderTypes.Select(pgt => pgt.GenderType.GenderName).ToList(),
                    PictureUrl = a.PicturesUrl
                        .GroupBy(x => x.ColorId)
                        .Select(g => new ImageDTOResponse
                        {
                            PictureDTO = g.Select(p => new PictureDTO
                            {
                                ImageId = p.ColorId,
                                IsDefault = p.IsDefault,
                                ImageUrl = Path.Combine(baseUrl, "managersProductsPhotos", a.ManagerId.ToString(), a.Id.ToString(), p.Name)
                            }).ToList()
                        }).ToList(),
                    ProductName = a.ProductName,
                    Id = a.Id,
                    Price = a.Price,
                    Description = a.Description,
                    Brand = a.Brand.BrandName,
                    Category = a.Category.CategoryName,
                    FrameSize = a.FrameSize.ToString(),
                    FrameType = a.FrameType.FrameTypeName,
                    Shape = a.Shape.ShapeName,
                    ColorsNames = a.ProductColors.Select(pc => pc.Color.ColorName).ToList(),
                    AvailableQuantity = a.AvailableQuantity,
                    MostPopular = a.MostPopular,
                    IsFavorite = false,
                    ManagerId = a.ManagerId
                })
                .PaginatedListAsync(pageIndex, pageSize);

            return filteredGlasses;
        }



        public async Task<PaginatedList<LensesDTOResponse>> GetManagerLensesAsync(
    Guid managerId, LenseFilterDTO lenseFilterDTO, string baseUrl,
    int pageIndex = 1, int pageSize = 10)
        {
            var filteredLenses = await _dbContext.Lenses
                .Include(a => a.Brand)
                .Include(a => a.Category)
                .Include(a => a.FavoriteProducts)
                .Include(a => a.ProductGenderTypes)
                    .ThenInclude(pt => pt.GenderType)
                .Include(a => a.ProductColors)
                    .ThenInclude(pc => pc.Color)
                .Include(a => a.PicturesUrl)
                .Where(a => a.ManagerId == managerId) // Filter by managerId
                .Select(a => new LensesDTOResponse()
                {
                    Genders = a.ProductGenderTypes.Select(pgt => pgt.GenderType.GenderName).ToList(),
                    PictureUrl = a.PicturesUrl
                        .GroupBy(x => x.ColorId)
                        .Select(g => new ImageDTOResponse
                        {
                            PictureDTO = g.Select(p => new PictureDTO
                            {
                                ImageId = p.ColorId,
                                IsDefault = p.IsDefault,
                                ImageUrl = Path.Combine(baseUrl, "managersProductsPhotos", a.ManagerId.ToString(), a.Id.ToString(), p.Name)
                            }).ToList()
                        }).ToList(),
                    ProductName = a.ProductName,
                    Id = a.Id,
                    Price = a.Price,
                    Description = a.Description,
                    Brand = a.Brand.BrandName,
                    Category = a.Category.CategoryName,
                    LensBaseCurve = a.LensBaseCurve,
                    Lensdiameter = a.Lensdiameter,
                    LensUsage = a.LensUsage.ToString(),
                    WaterContent = a.WaterContent,
                    AvailableQuantity = a.AvailableQuantity,
                    MostPopular = a.MostPopular,
                    IsFavorite = false,
                })
                .PaginatedListAsync(pageIndex, pageSize);

            return filteredLenses;
        }



        public async Task<BaseEntity?> GetProductByIdAsync(Guid id, string baseUrl, Guid? userId)
        {
            var product = await _dbContext
                .Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                    .ThenInclude(c => c.SubCategories)
                .Include(p => ((Glass)p).Shape)
                .Include(p => ((Glass)p).FrameType)
                .Include(p => p.ProductGenderTypes)
                    .ThenInclude(p => p.GenderType)
                .Include(p => p.ProductColors)
                    .ThenInclude(p => p.Color)
                .Include(p => p.PicturesUrl)
                .Include(p => p.Reviews)
        
                .Include(p => p.FavoriteProducts)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product is null)
            {
                return null;
            }

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

            var colorsNames = product
                .ProductColors
                .Select(pc => pc.Color.ColorName)
                .ToList();

            if (product is Glass)
            {
                var glass = product as Glass;

                var genderIds = glass.ProductGenderTypes.Select(x => x.GenderTypeId).ToList();
                var Genders = await _dbContext.GenderTypes
                    .Where(g => genderIds.Contains(g.Id))
                    .Select(g => g.GenderName)
                    .ToListAsync();
                
                var glassDto = new GlassDTOResponse()
                {
                    ProductType = (int)ProductType.GLASS,
                    ProductName = product!.ProductName,
                    Id = product.Id,
                    Price = product.Price,
                    PictureUrl = imagesURLs,
                    Description = product.Description,
                    Brand = glass.Brand.BrandName,
                    Category = glass.Category.CategoryName,
                    Genders = Genders,
                    FrameSize = glass.FrameSize.ToString(),
                    FrameType = glass.FrameType.FrameTypeName,
                    Shape = glass.Shape.ShapeName,
                    ColorsNames = colorsNames,
                    MostPopular = glass.MostPopular,
                    AvailableQuantity = glass.AvailableQuantity,
                    ManagerId = product.ManagerId,
                    IsFavorite = userId != null ?
                        glass.FavoriteProducts.Any(x => x.ProductId == glass.Id && x.ApplicationUserId == userId) : false
                };
                return glassDto;
            }

            else if (product is Accessory)
            {
                var accessoryEntity = product as Accessory;

                var genderIds = accessoryEntity.ProductGenderTypes.Select(x => x.GenderTypeId).ToList();
                var Genders = await _dbContext.GenderTypes
                    .Where(g => genderIds.Contains(g.Id))
                    .Select(g => g.GenderName)
                    .ToListAsync();

                var accessoryDto = new AccessoryDTOResponse
                {
                    ProductType = (int)ProductType.ACCESSORY,
                    ManagerId = product.ManagerId,
                    ProductName = accessoryEntity!.ProductName,
                    Id = accessoryEntity.Id,
                    Price = accessoryEntity.Price,
                    PictureUrl = imagesURLs,
                    Description = accessoryEntity.Description,
                    Brand = accessoryEntity.Brand.BrandName,
                    Category = accessoryEntity.Category.CategoryName,
                    Genders = Genders,
                    ColorsNames = colorsNames,
                    AvailableQuantity = accessoryEntity.AvailableQuantity,
                    MostPopular = accessoryEntity.MostPopular,
                    IsFavorite = userId != null ?
                        accessoryEntity.FavoriteProducts.Any(x => x.ProductId == accessoryEntity.Id && x.ApplicationUserId == userId.Value) : false
                    
                };

                return accessoryDto;
            }

            else if (product is Lense)
            {
                var lenseEntity = product as Lense;

                var genderIds = lenseEntity.ProductGenderTypes.Select(x => x.GenderTypeId).ToList();
                var Genders = await _dbContext.GenderTypes
                    .Where(g => genderIds.Contains(g.Id))
                    .Select(g => g.GenderName)
                    .ToListAsync();

                var lenseDto = new LensesDTOResponse()
                {
                    ProductType = (int)ProductType.LENSE,
                    ManagerId = product.ManagerId,
                    ProductName = lenseEntity!.ProductName,
                    Id = lenseEntity.Id,
                    Price = lenseEntity.Price,
                    PictureUrl = imagesURLs,
                    Description = lenseEntity.Description,
                    Brand = lenseEntity.Brand.BrandName,
                    Category = lenseEntity.Category.CategoryName,
                    Genders = Genders,
                    LensBaseCurve = lenseEntity.LensBaseCurve,
                    Lensdiameter = lenseEntity.Lensdiameter,
                    LensUsage = lenseEntity.LensUsage.ToString(),
                    WaterContent = lenseEntity.WaterContent,
                    ColorsNames = colorsNames,
                    AvailableQuantity = lenseEntity.AvailableQuantity,
                    MostPopular = lenseEntity.MostPopular,
                    IsFavorite = userId != null ?
                        lenseEntity.FavoriteProducts.Any(x => x.ProductId == lenseEntity.Id && x.ApplicationUserId == userId.Value) : false
                };

                return lenseDto;
            }

            else
            {
                return null;
            }
        }



        public async Task<bool> DeleteProductAsync(Guid id)
        {
            Product? prodcut = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (prodcut is null)
                return false;

            // Remove product imager folder
            string folderPath = Path.Combine(_env.WebRootPath,
                "managersProductsPhotos", prodcut.ManagerId.ToString(),
                id.ToString());

            // Check if the folder exists before attempting to delete
            if (Directory.Exists(folderPath))
            {
                Directory.Delete(folderPath, true); // Recursive deletion
            }

            _dbContext.Products.Remove(prodcut);

            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<PaginatedList<AccessoryDTOResponse>>
GetAccessoriesAsync(
    AccessoryFilterDTO accessoryFiltered,
    string baseUrl,
    string? search,
    Guid? userId,
    int pageIndex = 1,
    int pageSize = 10)
        {
            var filteredAccessories = await _dbContext.Accessories
                .Include(a => a.Brand)
                .Include(a => a.Category)
                .Include(a => a.PicturesUrl)
                .Include(a => a.Reviews)
                .Include(a => a.FavoriteProducts)
                .Include(a => a.ProductColors)
                    .ThenInclude(c => c.Color)
                .Include(a => a.ProductGenderTypes)
                    .ThenInclude(g => g.GenderType)
                .Where(a => (!accessoryFiltered.MinPrice.HasValue && !accessoryFiltered.MaxPrice.HasValue) ||
                            (a.Price >= (accessoryFiltered.MinPrice ?? 0) &&
                            (!accessoryFiltered.MaxPrice.HasValue || a.Price <= accessoryFiltered.MaxPrice.Value)))
                .Where(a => accessoryFiltered.GendersId == null || a.ProductGenderTypes.Any(p => accessoryFiltered.GendersId.Contains(p.GenderTypeId)))
                .Where(a => (accessoryFiltered.CategoryIds == null || accessoryFiltered.CategoryIds.Contains(a.CategoryId)))
                .Where(a => (accessoryFiltered.BrandIds == null || accessoryFiltered.BrandIds.Contains(a.BrandId)))
                .Where(a => (string.IsNullOrEmpty(search) || a.ProductName.ToLower().Contains(search)))
                .Select(a => new AccessoryDTOResponse
                {
                    Genders = a.ProductGenderTypes.Select(pgt => pgt.GenderType.GenderName).ToList(),
                    PictureUrl = a.PicturesUrl
                        .GroupBy(x => x.ColorId)
                        .Select(g => new ImageDTOResponse
                        {
                            ColorId = g.Key,
                            PictureDTO = g.Select(p => new PictureDTO
                            {
                                ImageId = p.Id,
                                IsDefault = p.IsDefault,
                                ImageUrl = Path.Combine(baseUrl, "managersProductsPhotos", a.ManagerId.ToString(), a.Id.ToString(), p.Name)
                            }).ToList()
                        }).ToList(),
                    ProductName = a!.ProductName,
                    Id = a.Id,
                    Price = a.Price,
                    Description = a.Description,
                    Brand = a.Brand.BrandName,
                    Category = a.Category.CategoryName,
                    ProductType = (int)ProductType.ACCESSORY,
                    ColorsNames = a.ProductColors.Select(pc => pc.Color.ColorName).ToList(),
                    AvailableQuantity = a.AvailableQuantity,
                    MostPopular = a.MostPopular,
                    IsFavorite = userId != null ? a.FavoriteProducts.Any(x => x.ProductId == a.Id && x.ApplicationUserId == userId) : false,
                    ManagerId = a.ManagerId,
                })
                .PaginatedListAsync(pageIndex, pageSize);

            return filteredAccessories;
        }


        public async Task<PaginatedList<GlassDTOResponse>> GetGlassesAsync(
     GlassFilterDTO glassFilterDTO, string baseUrl,
     string? search, Guid? userId, int pageIndex = 1, int pageSize = 10)
        {
            var filteredAccessories = await _dbContext.Glasses
                .Include(a => a.Brand)
                .Include(a => a.Category)
                .Include(a => a.ProductGenderTypes)
                    .ThenInclude(pg => pg.GenderType)
                .Include(a => a.FrameType)
                .Include(a => a.Shape)
                .Include(a => a.ProductColors)
                    .ThenInclude(pg => pg.Color)
                .Include(a => a.PicturesUrl)
                .Include(a => a.FavoriteProducts)
                .Where(a => (!glassFilterDTO.MinPrice.HasValue && !glassFilterDTO.MaxPrice.HasValue) ||
                            (a.Price >= (glassFilterDTO.MinPrice ?? 0) &&
                            (!glassFilterDTO.MaxPrice.HasValue || a.Price <= glassFilterDTO.MaxPrice.Value)))
                .Where(a => glassFilterDTO.GendersId == null || a.ProductGenderTypes.Any(p => glassFilterDTO.GendersId.Contains(p.GenderTypeId)))
                .Where(a => glassFilterDTO.CategoryIds == null || glassFilterDTO.CategoryIds.Contains(a.CategoryId))
                .Where(a => glassFilterDTO.BrandIds == null || glassFilterDTO.BrandIds.Contains(a.BrandId))
                .Where(a => glassFilterDTO.ShapeIds == null || glassFilterDTO.ShapeIds.Contains(a.ShapeID))
                .Where(a => glassFilterDTO.FrameTypeIds == null || glassFilterDTO.FrameTypeIds.Contains(a.FrameTypeID))
                .Where(a => glassFilterDTO.FrameSizes == null || glassFilterDTO.FrameSizes.Contains((int)a.FrameSize))
                .Where(a => string.IsNullOrEmpty(search) || a.ProductName.ToLower().Contains(search))
                .Select(a => new GlassDTOResponse
                {
                    Genders = a.ProductGenderTypes.Select(pgt => pgt.GenderType.GenderName).ToList(),
                    PictureUrl = a.PicturesUrl
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
                                    a.ManagerId.ToString(),
                                    a.Id.ToString(),
                                    p.Name)
                            }).ToList()
                        }).ToList(),
                    ProductName = a.ProductName,
                    Id = a.Id,
                    Price = a.Price,
                    Description = a.Description,
                    Brand = a.Brand.BrandName,
                    Category = a.Category.CategoryName,
                    FrameSize = a.FrameSize.ToString(),
                    FrameType = a.FrameType.FrameTypeName,
                    Shape = a.Shape.ShapeName,
                    ColorsNames = a.ProductColors
                        .Select(pc => pc.Color.ColorName)
                        .ToList(),
                    ProductType = (int)ProductType.GLASS,
                    AvailableQuantity = a.AvailableQuantity,
                    ManagerId = a.ManagerId,
                    MostPopular = a.MostPopular,
                    IsFavorite = userId != null
                        ? a.FavoriteProducts.Any(x => x.ProductId == a.Id && x.ApplicationUserId == userId.Value)
                        : false
                })
                .PaginatedListAsync(pageIndex, pageSize);

            return filteredAccessories;
        }


        public async Task<PaginatedList<LensesDTOResponse>>
            GetLensesAsync(LenseFilterDTO lenseFilterDTO, 
            string baseUrl,
            string? search, Guid? userId,
            int pageIndex = 1,
            int pageSize = 10)
        {

            var filteredLenses = await _dbContext.Lenses
                .Include(a => a.Brand)
                .Include(a => a.Category)
                .Include(a => a.FavoriteProducts)
                .Include(a => a.ProductGenderTypes)
                    .ThenInclude(pt => pt.GenderType)
                .Include(a => a.ProductColors)
                    .ThenInclude(pc => pc.Color)
                .Include(a => a.PicturesUrl)
                .Where(a => (!lenseFilterDTO.MinPrice.HasValue && !lenseFilterDTO.MaxPrice.HasValue) ||
                            (a.Price >= (lenseFilterDTO.MinPrice ?? 0) &&
                            (!lenseFilterDTO.MaxPrice.HasValue || a.Price <= lenseFilterDTO.MaxPrice.Value)))

                .Where(a => lenseFilterDTO.GendersId == null || a.ProductGenderTypes.Any(p => lenseFilterDTO.GendersId.Contains(p.GenderTypeId)))
                .Where(a => (lenseFilterDTO.CategoryIds == null || lenseFilterDTO.CategoryIds.Contains(a.CategoryId)))
                .Where(a => (lenseFilterDTO.BrandIds == null || lenseFilterDTO.BrandIds.Contains(a.BrandId)))
                .Where(a => (lenseFilterDTO.WaterContent == null || lenseFilterDTO.WaterContent == a.WaterContent))
                .Where(a => (lenseFilterDTO.LensBaseCurve == null || lenseFilterDTO.LensBaseCurve == a.LensBaseCurve))
                .Where(a => (lenseFilterDTO.LensUsage == null || (LensUsage)lenseFilterDTO.LensUsage == a.LensUsage))
                .Where(a => (lenseFilterDTO.Lensdiameter == null || lenseFilterDTO.Lensdiameter == a.Lensdiameter))
                .Where(a => (string.IsNullOrEmpty(search) || a.ProductName.ToLower().Contains(search)))
                .Where(a => (string.IsNullOrEmpty(search) || a.ProductName.ToLower().Contains(search)))
                .Select(a => new LensesDTOResponse()
                {
                    Genders = a.ProductGenderTypes.Select(pgt => pgt.GenderType.GenderName).ToList(),
                    PictureUrl = a.PicturesUrl
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
                                    a.ManagerId.ToString(),
                                    a.Id.ToString(),
                                    p.Name)
                            }).ToList()
                        }).ToList(),
                    ProductName = a!.ProductName,
                    ManagerId = a.ManagerId, 
                    Id = a.Id,
                    Price = a.Price,
                    Description = a.Description,
                    Brand = a.Brand.BrandName,
                    Category = a.Category.CategoryName,
                    LensBaseCurve = a.LensBaseCurve,
                    Lensdiameter = a.Lensdiameter,
                    LensUsage = a.LensUsage.ToString(),
                    WaterContent = a.WaterContent,
                    ColorsNames = a.ProductColors
                            .Select(pc => pc.Color.ColorName)
                            .ToList(),
                    ProductType = (int)ProductType.LENSE,
                    AvailableQuantity = a.AvailableQuantity,
                    MostPopular = a.MostPopular,
                    IsFavorite = userId != null ? 
                    a.FavoriteProducts.Any(x => x.ProductId == a.Id 
                    &&
               x.ApplicationUserId == userId) : false
                })
                .PaginatedListAsync(pageIndex, pageSize);


            return filteredLenses;
        }

        public async Task<PaginatedList<BaseEntity>?> Search(
     string baseUrl, string? search, Guid? userId,
     int pageIndex = 1, int pageSize = 10)
        {
            if (string.IsNullOrEmpty(search))
            {
                return null;
            }

            search = search.ToLower();

            var query = _dbContext.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                    .ThenInclude(c => c.SubCategories)
                .Include(p => p.ProductGenderTypes)
                    .ThenInclude(pgt => pgt.GenderType)
                .Include(p => p.ProductColors)
                    .ThenInclude(pc => pc.Color)
                .Include(p => p.PicturesUrl)
                .Include(p => p.Reviews)
                .Include(p => ((Glass)p).Shape)
                .Include(p => ((Glass)p).FrameType)
                .AsQueryable();

            query = query.Where(a =>
                a.Brand.BrandName.ToLower().Contains(search) ||
                a.Category.CategoryName.ToLower().Contains(search) ||
                a.ProductName.ToLower().Contains(search));

            var productsList = await query.ToListAsync();

            var response = new List<BaseEntity>();

            foreach (var product in productsList)
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

                var colorsNames = product.ProductColors
                    .Select(pc => pc.Color.ColorName)
                    .ToList();

                var genders = await _dbContext.GenderTypes
                    .Where(gt => product.ProductGenderTypes.Select(pgt => pgt.GenderTypeId).Contains(gt.Id))
                    .Select(gt => gt.GenderName)
                    .ToListAsync();

                if (product is Accessory accessory)
                {
                    var accessoryDto = new AccessoryDTOResponse
                    {
                        ProductName = accessory.ProductName,
                        Id = accessory.Id,
                        Price = accessory.Price,
                        PictureUrl = imagesURLs,
                        Description = accessory.Description,
                        Brand = accessory.Brand.BrandName,
                        Category = accessory.Category.CategoryName,
                        Genders = genders,
                        ColorsNames = colorsNames,
                        AvailableQuantity = accessory.AvailableQuantity,
                        MostPopular = accessory.MostPopular,
                        ProductType = (int)ProductType.ACCESSORY,
                        IsFavorite = userId != null && accessory.FavoriteProducts.Any(x => x.ProductId == accessory.Id && x.ApplicationUserId == userId.Value)
                    };

                    response.Add(accessoryDto);
                }
                else if (product is Lense lense)
                {
                    var lenseDto = new LensesDTOResponse
                    {
                        ProductName = lense.ProductName,
                        Id = lense.Id,
                        Price = lense.Price,
                        PictureUrl = imagesURLs,
                        Description = lense.Description,
                        Brand = lense.Brand.BrandName,
                        Category = lense.Category.CategoryName,
                        Genders = genders,
                        LensBaseCurve = lense.LensBaseCurve,
                        Lensdiameter = lense.Lensdiameter,
                        LensUsage = lense.LensUsage.ToString(),
                        WaterContent = lense.WaterContent,
                        ColorsNames = colorsNames,
                        AvailableQuantity = lense.AvailableQuantity,
                        MostPopular = lense.MostPopular,
                        ProductType = (int)ProductType.LENSE,
                        IsFavorite = userId != null && lense.FavoriteProducts.Any(x => x.ProductId == lense.Id && x.ApplicationUserId == userId.Value)
                    };

                    response.Add(lenseDto);
                }
                else if (product is Glass glass)
                {
                    var glassDto = new GlassDTOResponse
                    {
                        ProductName = glass.ProductName,
                        Id = glass.Id,
                        Price = glass.Price,
                        PictureUrl = imagesURLs,
                        Description = glass.Description,
                        Brand = glass.Brand.BrandName,
                        Category = glass.Category.CategoryName,
                        Genders = genders,
                        FrameSize = glass.FrameSize.ToString(),
                        FrameType = glass.FrameType.FrameTypeName,
                        Shape = glass.Shape.ShapeName,
                        ColorsNames = colorsNames,
                        AvailableQuantity = glass.AvailableQuantity,
                        MostPopular = glass.MostPopular,
                        ProductType = (int)ProductType.GLASS,
                        IsFavorite = userId != null && glass.FavoriteProducts.Any(x => x.ProductId == glass.Id && x.ApplicationUserId == userId.Value)
                    };

                    response.Add(glassDto);
                }
            }

            var paginatedResponse = new PaginatedList<BaseEntity>(
                response,
                response.Count, pageIndex, pageSize);

            return paginatedResponse;
        }



        public async Task<bool> UpdateAccessory(
            AccessoryUpdateDTO product, string baseUrl)
        {
            var accessory = await
                _dbContext
                .Accessories
                .Include(x => x.PicturesUrl)
                .Include(x => x.Brand)
                .Include(x => x.Category)
                    .ThenInclude(x => x.SubCategories)
                .Include(x => x.ProductGenderTypes)
                .Include(x => x.PicturesUrl)
                .Include(x => x.ProductColors)
                .FirstOrDefaultAsync(x => x.Id == product.Id);

            // 1- update the pictures in wwwroot
            // 2- update the pictures in the database


            if (accessory is null)
                return false;

            accessory.Price = product.Price;
            accessory.CategoryId = product.CategoryId;
            accessory.BrandId = product.BrandId;
            accessory.ProductName = product.ProductName;
            accessory.Description = product.Description;
            // accessory.PictureUrl = product.PictureUrl;

            accessory.ProductGenderTypes.Clear();


            // 1- update the pictures in wwwroot by removing them

            var folderPath = Path.Combine(_env.WebRootPath,
                "managersProductsPhotos",
                accessory.ManagerId.ToString(),
                accessory.Id.ToString());

            if (Directory.Exists(folderPath))
            {
                Directory.Delete(folderPath, true);
            }



            // create the product folder again

            Directory.CreateDirectory(folderPath);


            // 2- create the folders again in database
            List<Image> images = new List<Image>();

            foreach (var image in product.PicturesUrl)
            {
                Guid imageUnique = Guid.NewGuid();
                var photoPath = Path.Combine(folderPath
                    , $"{imageUnique + "_" + image.image.FileName}");
                using (var memoryStream = new MemoryStream())
                {
                    await image.image.CopyToAsync(memoryStream);
                    // memoryStream.Position = 0;
                    await System.IO.File.WriteAllBytesAsync(photoPath, memoryStream.ToArray());
                }

                var img = new Image
                {
                    Name = $"{imageUnique + "_" + image.image.FileName}",
                    ProductId = accessory.Id,
                    ColorId = image.ColorId,
                    IsDefault = image.IsDefault
                };

                images.Add(img);
            }


            accessory.PicturesUrl = images;


            var genderIds = product.GendersId.Select(g => g.GenderId).ToList();

            var existingGenderTypes = await _dbContext.GenderTypes
                .Where(gt => genderIds.Contains(gt.Id))
                .ToListAsync();

            foreach (var genderId in product.GendersId)
            {
                var genderType = existingGenderTypes
                    .FirstOrDefault(gt => gt.Id == genderId.GenderId);

                if (genderType != null)
                {
                    accessory.ProductGenderTypes.Add(new ProductGenderType
                    {
                        Product = accessory,
                        GenderType = genderType
                    });
                }
            }

            accessory.ProductColors.Clear();

            var colorIds = product.PicturesUrl.Select(c => c.ColorId).ToList();

            var existingColors = await _dbContext.Colors
                .Where(c => colorIds.Contains(c.Id))
                .ToListAsync();

            foreach (var colorId in colorIds)
            {
                var color = existingColors
                    .FirstOrDefault(c => c.Id == colorId);

                if (color != null)
                {
                    accessory.ProductColors.Add(new ProductColor
                    {
                        Product = accessory,
                        Color = color
                    });
                }
            }


            await _dbContext.SaveChangesAsync();

            return true;

        }

        public async Task<bool> UpdateGlass(GlassUpdateDTO product,
            string baseUrl)
        {
            var glass = await
                _dbContext
                .Glasses
                .Include(x => x.PicturesUrl)
                .Include(x => x.Brand)
                .Include(x => x.Category)
                    .ThenInclude(x => x.SubCategories)
                .Include(x => x.ProductGenderTypes)
                .Include(x => x.ProductColors)
                .Include(x => x.FrameType)
                .FirstOrDefaultAsync(x => x.Id == product.Id);

            if (glass is null)
                return false;

            glass.Price = product.Price;
            glass.CategoryId = product.CategoryId;
            glass.BrandId = product.BrandId;
            glass.ProductName = product.ProductName;
            glass.Description = product.Description;
            //glass.PictureUrl = product.PictureUrl;
            glass.FrameTypeID = product.FrameTypeId;
            glass.ShapeID = product.ShapeId;
            glass.FrameSize = (FrameSize)product.FrameSize;

            glass.ProductGenderTypes.Clear();

            var folderPath = Path.Combine(_env.WebRootPath,
                "managersProductsPhotos",
                glass.ManagerId.ToString(),
                glass.Id.ToString());


            Directory.Delete(folderPath, true);


            // create the product folder again

            Directory.CreateDirectory(folderPath);


            // 2- create the folders again in database
            List<Image> images = new List<Image>();

            foreach (var image in product.PicturesUrl)
            {
                Guid imageUnique = Guid.NewGuid();
                var photoPath = Path.Combine(folderPath,
                    $"{imageUnique + "_" + image.image.FileName}"
                    );
                using (var memoryStream = new MemoryStream())
                {
                    await image.image.CopyToAsync(memoryStream);
                    // memoryStream.Position = 0;
                    await System.IO.File.WriteAllBytesAsync(
                        photoPath, memoryStream.ToArray());
                }

                var img = new Image
                {
                    Name = $"{imageUnique + "_" + image.image.FileName}",
                    ProductId = glass.Id,
                    ColorId = image.ColorId,
                    IsDefault = image.IsDefault
                };

                images.Add(img);
            }


            glass.PicturesUrl = images;

            foreach (var genderId in product.GendersId)
            {
                var genderType = await _dbContext.GenderTypes.FindAsync(genderId.GenderId);

                if (genderType != null)
                {
                    glass.ProductGenderTypes.Add(new ProductGenderType
                    {
                        Product = glass,
                        GenderType = genderType
                    });
                }
            }

            glass.ProductColors.Clear();

            var colorIds = product.PicturesUrl.Select(c => c.ColorId).ToList();

            foreach (var colorId in colorIds)
            {
                var color = await _dbContext.Colors
                    .FindAsync(
                    colorId);

                if (color != null)
                {
                    glass.ProductColors.Add(new ProductColor
                    {
                        Product = glass,
                        Color = color
                    });
                }
            }


            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateLense(LenseUpdateDTO product, string baseUrl)
        {
            var lense = await
            _dbContext
            .Lenses
            .Include(x => x.PicturesUrl)
            .Include(x => x.Brand)
            .Include(x => x.Category)
                .ThenInclude(x => x.SubCategories)
            .Include(x => x.ProductGenderTypes)
            .Include(x => x.ProductColors)
            .Include(x => x.PicturesUrl)
            .FirstOrDefaultAsync(x => x.Id == product.Id);

            if (lense is null)
                return false;

            lense.Price = product.Price;
            lense.CategoryId = product.CategoryId;
            lense.BrandId = product.BrandId;
            lense.ProductName = product.ProductName;
            lense.Description = product.Description;
            //lense.PictureUrl = product.PictureUrl;
            lense.Lensdiameter = product.Lensdiameter;
            lense.LensUsage = (LensUsage)product.LensUsage;
            lense.LensBaseCurve = product.LensBaseCurve;
            lense.WaterContent = product.WaterContent;


            lense.ProductGenderTypes.Clear();

            var folderPath = Path.Combine(_env.WebRootPath,
                "managersProductsPhotos", lense.ManagerId.ToString(), lense.Id.ToString());

            Directory.Delete(folderPath, true);


            // create the product folder again

            Directory.CreateDirectory(folderPath);



            // 2- create the folders again in database
            List<Image> images = new List<Image>();

            foreach (var image in product.PicturesUrl)
            {
                Guid imageUnique = Guid.NewGuid();
                var photoPath = Path.Combine(folderPath, $"{imageUnique + "_" + image
                    .image.FileName}");
                using (var memoryStream = new MemoryStream())
                {
                    await image.image.CopyToAsync(memoryStream);
                    // memoryStream.Position = 0;
                    await System.IO.File.WriteAllBytesAsync(photoPath, memoryStream.ToArray());
                }

                var img = new Image
                {
                    Name = $"{imageUnique + "_" + image.image.FileName}",
                    ProductId = lense.Id,
                    ColorId = image.ColorId,
                    IsDefault = image.IsDefault
                };
                images.Add(img);
            }


            lense.PicturesUrl = images;


            foreach (var genderId in product.GendersId)
            {
                var genderType = await _dbContext.GenderTypes.FindAsync(genderId.GenderId);

                if (genderType != null)
                {
                    lense.ProductGenderTypes.Add(new ProductGenderType
                    {
                        Product = lense,
                        GenderType = genderType
                    });
                }
            }

            lense.ProductColors.Clear();

            var colorIds = product.PicturesUrl.Select(c => c.ColorId).ToList();

            foreach (var colorId in product.ColorsId)
            {
                var color = await _dbContext.Colors
                    .FindAsync(
                    colorId.Colorid);

                if (color != null)
                {
                    lense.ProductColors.Add(new ProductColor
                    {
                        Product = lense,
                        Color = color
                    });
                }
            }



            await _dbContext.SaveChangesAsync();


            return true;
        }



        public async Task<IReadOnlyList<BaseEntity>> GetBrandsAsync()
        {
            return await _dbContext.Brands.ToListAsync();
        }

        public async Task<BaseEntity?> AddBrand(string englishName)
        {
            if (englishName == null) return null;

            var newBrand = new Brand() { BrandName = englishName };

            await _dbContext.Brands.AddAsync(newBrand);
            _dbContext.SaveChanges();

            return newBrand;
        }

        public async Task<bool?> DeleteBrandById(Guid id)
        {

            var brand = await _dbContext.Brands.FirstOrDefaultAsync(b => b.Id == id);
            if (brand == null) return false;

            _dbContext.Brands.Remove(brand);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        //public async Task<IReadOnlyList<BaseEntity>> GetCategoriesAsync()
        //{
        //    return await _dbContext
        //        .Categories
        //        .Include(c => c.CategoriesTypes)
        //        .ToListAsync();
        //}


        public async Task<bool?> DeleteCategoryById(Guid id)
        {
            var category = await _dbContext.Categories.FirstOrDefaultAsync(b => b.Id == id);

            if (category == null) return false;

            _dbContext.Categories.Remove(category);

            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<IReadOnlyList<BaseEntity>> GetGenders()
        {
            return await _dbContext.GenderTypes.ToListAsync();
        }

        // Create Glass
        public async Task<Guid?> CreateGlassAsync(GlassDtoRequest product, Guid managerId)
        {
            var productID = Guid.NewGuid();
            var glass = _mapper.Map<GlassDtoRequest, Glass>(product);
            glass.ManagerId = managerId;
            glass.Id = productID;

            // Add gender types to the glass
            foreach (var genderId in product.GendersId)
            {
                var gender = await _dbContext.GenderTypes.FirstOrDefaultAsync(p => p.Id == genderId.GenderId);
                if (gender != null)
                {
                    glass.ProductGenderTypes.Add(new ProductGenderType { Product = glass, GenderType = gender });
                }
            }

            // Handle product colors from images
            var colorsFromImages = product.PicturesUrl.Select(x => x.ColorId).Distinct().ToList();
            foreach (var colorId in colorsFromImages)
            {
                var color = await _dbContext.Colors.FirstOrDefaultAsync(c => c.Id == colorId);
                if (color != null)
                {
                    var existingProductColor = await _dbContext.ProductColors.FirstOrDefaultAsync(pc => pc.ProductId == productID && pc.ColorId == colorId);
                    if (existingProductColor == null)
                    {
                        glass.ProductColors.Add(new ProductColor { ProductId = productID, ColorId = colorId });
                    }
                }
            }

            // Add images to www root and handle image table
            var folderPath = Path.Combine(_env.WebRootPath, "managersProductsPhotos", managerId.ToString(), productID.ToString());
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            List<Image> images = new List<Image>();
            foreach (var image in product.PicturesUrl)
            {
                Guid imageUnique = Guid.NewGuid();
                var photoPath = Path.Combine(folderPath, $"{imageUnique}_{image.image.FileName}");
                using (var memoryStream = new MemoryStream())
                {
                    await image.image.CopyToAsync(memoryStream);
                    await System.IO.File.WriteAllBytesAsync(photoPath, memoryStream.ToArray());
                }

                var img = new Image
                {
                    Name = $"{imageUnique}_{image.image.FileName}",
                    ProductId = productID,
                    ColorId = image.ColorId,
                    IsDefault = image.IsDefault
                };

                images.Add(img);
            }

            glass.PicturesUrl = images;

            // Add the glass object to the context
            await _dbContext.Glasses.AddAsync(glass);

            // Attach images to the context
            await _dbContext.Images.AddRangeAsync(images);

            // Save changes to the database
            await _dbContext.SaveChangesAsync();

            return productID;
        }


        public async Task<Guid?> CreateAccessoryAsync(
            AccessoryDtoRequest product, Guid managerId)
        {
            var productID = Guid.NewGuid();

            if (product is null)
                return null;

            var accessory = new Accessory();

            accessory = _mapper.Map(product, accessory);

            foreach (var genderId in product.GendersId)
            {
                var gender = await
                    _dbContext.GenderTypes.FindAsync(genderId.GenderId);

                if (gender != null)
                {
                    accessory.ProductGenderTypes.Add(new ProductGenderType
                    { Product = accessory, GenderType = gender });
                }
            }

            var colorsFromImages =
                product.PicturesUrl.Select(x => x.ColorId).ToList();


            foreach (var colorId in colorsFromImages)
            {
                var color = await _dbContext.Colors
                    .FindAsync(
                    colorId);

                if (color != null)
                {
                    accessory.ProductColors.Add(new ProductColor
                    {
                        Product = accessory,
                        Color = color
                    });
                }
            }




            accessory.ManagerId = managerId;
            accessory.Id = productID;

            // add images to www root
            var folderPath = Path.Combine(_env.WebRootPath, "managersProductsPhotos",
                managerId.ToString(), productID.ToString());

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            List<Image> images = new List<Image>();

            foreach (var image in product.PicturesUrl)
            {
                Guid imageUnique = Guid.NewGuid();
                var photoPath = Path.Combine(folderPath,
                    $"{imageUnique + "_" + image.image.FileName}");
                using (var memoryStream = new MemoryStream())
                {
                    await image.image.CopyToAsync(memoryStream);
                    await System.IO.File.WriteAllBytesAsync(photoPath, memoryStream.ToArray());
                }

                var img = new Image
                {
                    Name = $"{imageUnique + "_" + image.image.FileName}",
                    ProductId = productID,
                    ColorId = image.ColorId,
                    IsDefault = image.IsDefault,
                };

                images.Add(img);
            }

            accessory.PicturesUrl = images;

            await _dbContext.Products.AddAsync(accessory);

            await _dbContext.SaveChangesAsync();

            return productID;

        }

        public async Task<Guid?> CreateLenseAsync(
            LensDtoRequest product, Guid managerId)
        {
            if (product is null)
                return null;

            var productID = Guid.NewGuid();
            var lens = new Lense();

            lens = _mapper.Map(product, lens);

            foreach (var genderId in product.GendersId)
            {
                var gender = await _dbContext.GenderTypes.FindAsync(genderId.GenderId);

                if (gender != null)
                {
                    lens.ProductGenderTypes.Add(new ProductGenderType
                    { Product = lens, GenderType = gender });
                }
            }

            var colorsFromImages =
                product.PicturesUrl.Select(x => x.ColorId).ToList();

            foreach (var colorId in colorsFromImages)
            {
                var color = await _dbContext.Colors
                    .FindAsync(
                    colorId);

                if (color != null)
                {
                    lens.ProductColors.Add(new ProductColor
                    {
                        Product = lens,
                        Color = color
                    });
                }
            }

            lens.ManagerId = managerId;
            lens.Id = productID;

            // add images to www root
            var folderPath = Path.Combine(_env.WebRootPath,
                "managersProductsPhotos", managerId.ToString(),
                productID.ToString()
                );

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            List<Image> images = new List<Image>();

            foreach (var image in product.PicturesUrl)
            {
                Guid imageUnique = Guid.NewGuid();
                var photoPath = Path.Combine(folderPath, $"{imageUnique + "_" +
                    image.image.FileName}");
                using (var memoryStream = new MemoryStream())
                {
                    await image.image.CopyToAsync(memoryStream);
                    await System.IO.File.WriteAllBytesAsync(photoPath, memoryStream.ToArray());
                }

                var img = new Image
                {
                    Name = $"{imageUnique + "_" + image.image.FileName}",
                    ProductId = productID,
                    ColorId = image.ColorId,
                    IsDefault = image.IsDefault,
                };

                images.Add(img);
            }

            lens.PicturesUrl = images;

            await _dbContext.Products.AddAsync(lens);
            await _dbContext.SaveChangesAsync();

            return productID;
        }


      

        public async Task<Color> CreateColor(string color)
        {
            Color colorCreated = new Color { ColorName = color };

            await _dbContext.Colors.AddAsync(colorCreated);

            await _dbContext.SaveChangesAsync();

            return colorCreated;
        }

     

    }
}

