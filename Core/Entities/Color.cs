using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Color : BaseEntity
    {
        public string ColorName { get; set; }

        public List<ProductColor> ProductColors { get; set; 
        }
    }
}