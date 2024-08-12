using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Review : BaseEntity
    {
        public string? Comment { get; set; }
        public double? Rating { get; set; }
        
        
        // relationship with the product , 
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
    }
}
