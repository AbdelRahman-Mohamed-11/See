using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Order
{
    public class ProductItemOrdered  // owned by order
    {
        public ProductItemOrdered() { }
        public ProductItemOrdered(Guid productItemId, string productName, string productImage)
        {
            ProductItemId = productItemId;
            ProductName = productName;
            ProductImage = productImage;
        }

        public Guid ProductItemId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage{ get; set; }
    }
}
