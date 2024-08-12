using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class GlassesRequest
    {
        public IFormFile FaceImage { get; set; }
        public Guid GlassImageId { get; set; }
    }

}
