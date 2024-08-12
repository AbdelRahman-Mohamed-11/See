using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Microsoft.AspNetCore.Http;

namespace Core.DTOS.ProductDTOS
{
    public class GlassDtoRequest
    {
        
        public string ProductName { get; set; }


        public string Description { get; set; }

        public List<ImageDTO> PicturesUrl { get; set; }

        public decimal Price { get; set; }


        public List<ProductGenderTypeDTO> GendersId { get; set; }

        public Guid CategoryId { get; set; }


        public Guid BrandId { get; set; }


        public Guid ShapeId { get; set; }  // frameshape

        public Guid FrameTypeId { get; set; }   // frametype

        public int FrameSize { get; set; }  // frameSize 0=>large , 1=> medium , 2=>small

    }
}