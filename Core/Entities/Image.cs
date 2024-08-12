using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Image : BaseEntity
    {
        public string Name { get; set; }

        public Product Product { get; set; }
        public Guid ProductId { get; set; }

        public Color color { get; set; }
        public Guid ColorId { get; set; }

        public bool IsDefault { get; set; }
    }
}
