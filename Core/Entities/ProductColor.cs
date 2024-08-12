using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ProductColor
    {
        public Guid ProductId { get; set; }
        public Product Product { get; set; }

        public Guid ColorId { get; set; }
        public Color Color { get; set; }
    }
}
