using Core.Entities;
using Core.Products;
using Microsoft.AspNetCore.Http;

namespace Core.DTOS.ProductDTOS
{
    public class LensDtoRequest
    {
        public string ProductName { get; set; }


        public string Description { get; set; }

        public List<ImageDTO> PicturesUrl { get; set; }

        public decimal Price { get; set; }

        public List<ProductGenderTypeDTO> GendersId { get; set; }

        public Guid CategoryId { get; set; }

        public Guid BrandId { get; set; }

        public double LensBaseCurve { get; set; }

        public double Lensdiameter { get; set; }

        public int LensUsage { get; set; }

        public double WaterContent { get; set; }

    }
}