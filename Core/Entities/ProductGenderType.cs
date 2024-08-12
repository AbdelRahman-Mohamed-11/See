using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ProductGenderType
    {
        public Guid ProductId { get; set; }
        public Product Product { get; set; }

        public Guid GenderTypeId { get; set; }
        public GenderType GenderType { get; set; }
    }
}
