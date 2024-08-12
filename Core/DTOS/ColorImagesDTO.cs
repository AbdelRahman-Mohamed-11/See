using Core.DTOS.ProductDTOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOS
{
    public class ColorImagesDTO
    {
        public Guid ColorId { get; set; }
        public string ColorName { get; set; }
        public List<ImageDTOResponse> Images { get; set; }
    }
}
