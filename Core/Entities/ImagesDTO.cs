using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ImageDTO
    {
        public IFormFile image { get; set; }
        
        public Guid ColorId { get; set; }

        public bool IsDefault { get; set; }
    }
}
