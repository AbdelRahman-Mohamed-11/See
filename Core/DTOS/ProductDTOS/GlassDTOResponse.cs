using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOS.ProductDTOS
{
    public class GlassDTOResponse : BaseEntity
    {
        public string ProductName { get; set; }

        public string Description { get; set; }

        public List<ImageDTOResponse> PictureUrl { get; set; }

        public List<string> ColorsNames { get; set; }

        public decimal Price { get; set; }


        public List<string> Genders { get; set; }

        
        public string Category { get; set; }


        public string Brand { get; set; }

        //if the product is glass
        public string Shape { get; set; }  // frameshape

        public string FrameType { get; set; }   // frametype

        public string FrameSize { get; set; }  // frameSize

        public int ProductType { get; set; }

        public int AvailableQuantity { get; set; }

        public int MostPopular { get; set; }

        public bool IsFavorite { get; set; }

        public Guid ManagerId { get; set; }
    }
}
