using Core.Entities;
using Core.Products;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOS.ProductDTOS.UpdateDTOS
{
    public class LenseUpdateDTO : BaseEntity
    {
        public string ProductName { get; set; }


        public string Description { get; set; }

        public List<ImageDTO> PicturesUrl { get; set; }

        public decimal Price { get; set; }

        public List<ProductGenderTypeDTO> GendersId { get; set; }

        public List<ProductColorDTO> ColorsId { get; set; }

        public Guid CategoryId { get; set; }


        public Guid BrandId { get; set; }

        public double LensBaseCurve { get; set; }

        public double Lensdiameter { get; set; }

        public int LensUsage { get; set; }

        public double WaterContent { get; set; }

    }
}
