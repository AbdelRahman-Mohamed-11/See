using Core.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Shape : BaseEntity
    {
        public string ShapeName { get; set; }

        public List<Glass> Glasses { get; set; }
    }
}
