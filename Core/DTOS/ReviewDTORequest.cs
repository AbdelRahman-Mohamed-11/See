using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOS
{
    public class ReviewDTORequest 
    {
        public string? Comment { get; set; }
        public int? Rating { get; set; }

        public Guid ProductId { get; set; }
    }
}
