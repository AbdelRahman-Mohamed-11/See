using Core.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOS.ProductDTOS.UpdateDTOS
{
    public class GlassUpdateDTO : BaseEntity
    {
        public string ProductName { get; set; }

        public string Description { get; set; }

        public List<ImageDTO> PicturesUrl { get; set; }

        public decimal Price { get; set; }


        public List<ProductGenderTypeDTO> GendersId { get; set; }

        public Guid CategoryId { get; set; }


        public Guid BrandId { get; set; }

        //if the product is glass
        public Guid ShapeId { get; set; }  // frameshape

        public Guid FrameTypeId { get; set; }   // frametype

        public int FrameSize { get; set; }  // frameSize 0=>large , 1=> medium , 2=>small

    }
}
