using Application.pagination;
using Application.Services;
using Core.DTOS;
using Core.DTOS.filteredDTO;
using Core.DTOS.ProductDTOS;
using Core.DTOS.ProductDTOS.UpdateDTOS;
using Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IProductService
    {
        public Task<bool> UpdateGlassImages(GlassUpdateImageDTO product, string baseUrl);
        public Task<PaginatedList<AccessoryDTOResponse>> GetManagerAccessoriesAsync(
    Guid managerId, AccessoryFilterDTO accessoryFilterDTO, string baseUrl,
    int pageIndex = 1, int pageSize = 10);

        public  Task<PaginatedList<GlassDTOResponse>> GetManagerGlassesAsync(
    Guid managerId, GlassFilterDTO glassFilterDTO, string baseUrl,
    int pageIndex = 1, int pageSize = 10);

        public Task<PaginatedList<LensesDTOResponse>> GetManagerLensesAsync(
    Guid managerId, LenseFilterDTO lenseFilterDTO, string baseUrl,
    int pageIndex = 1, int pageSize = 10);

        Task<PaginatedList<AccessoryDTOResponse>> GetAccessoriesAsync(
            AccessoryFilterDTO
            accessoryFiltered,string baseUrl, string? search,
            Guid? userId, 
            int pageIndex = 1, int pageSize = 10);

        Task<PaginatedList<GlassDTOResponse>> GetGlassesAsync(
            GlassFilterDTO glassFilterDTO, string baseUrl,
                string? search,
            Guid? userId,
            int pageIndex = 1,
            int pageSize = 10);

        Task<PaginatedList<LensesDTOResponse>> GetLensesAsync(
            LenseFilterDTO lenseFilterDTO , string baseUrl, 
            string? search, Guid? userId,
        int pageIndex = 1, int pageSize = 10);

        Task<BaseEntity?> GetProductByIdAsync(Guid id , string baseUrl
            , Guid? userId);



        // Task<BaseEntity> CreateProductAsync(ProductDTORequest product,string userId);
        Task<Guid?> CreateGlassAsync(GlassDtoRequest product,Guid managerId);
        Task<Guid?> CreateAccessoryAsync(
            AccessoryDtoRequest product
            , Guid managerId);
        Task<Guid?> CreateLenseAsync(LensDtoRequest product, Guid managerId);

        Task<bool> UpdateAccessory(AccessoryUpdateDTO product
            , string baseUrl);
        Task<bool> UpdateGlass(GlassUpdateDTO product, string baseUrl);
        Task<bool> UpdateLense(LenseUpdateDTO product, string baseUrl);

        Task<bool> DeleteProductAsync(Guid id);


        // Brands Methods
        Task<IReadOnlyList<BaseEntity>> GetBrandsAsync();
        
        Task<BaseEntity> AddBrand(string englishName);

        Task<bool?> DeleteBrandById(Guid id);

        //genders

        Task<IReadOnlyList<BaseEntity>> GetGenders();





        // Categories Methods



        Task<Color> CreateColor(string color);

        Task<bool?> DeleteCategoryById(Guid id);
        Task<PaginatedList<BaseEntity>?> Search(string baseUrl,string? search, Guid? userId,int pageIndex = 1, int pageSize = 10);
    }
}
