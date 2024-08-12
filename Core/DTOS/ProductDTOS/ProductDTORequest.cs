using Core.Entities;
using Core.enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOS.ProductDTOS
{
    public class ProductDTORequest
    {
        public string ProductName { get; set; }


        public string Description { get; set; }

        public List<IFormFile> PicturesUrls { get; set; }

        public decimal Price { get; set; }


        public List<ProductGenderTypeDTO> GendersId { get; set; }

        public List<ProductColorDTO> ColorsId { get; set; }

        public Guid CategoryId { get; set; }


        public Guid BrandId { get; set; }

        public Guid ManagerId { get; set; }

        //if the product is glass
        public Guid? ShapeId { get; set; }  // frameshape

        public Guid? FrameTypeId { get; set; }   // frametype

        public int? FrameSize { get; set; }  // frameSize 0=>large , 1=> medium , 2=>small

        // if the product is lenses
        public string? LensBaseCurve { get; set; }

        public string? Lensdiameter { get; set; }

        public string? LensUsage { get; set; }

        public string? WaterContent { get; set; }

    }
}
