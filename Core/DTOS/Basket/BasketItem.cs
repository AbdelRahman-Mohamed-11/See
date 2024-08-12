using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOS.Basket
{
    public class BasketItem
    {
        public Guid Id { get; set; }  // the id of the product

        public string ProductName { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public string PictureUrl { get; set; }

        public string Category { get; set; }

        public string Brand { get; set; }

        public string Color { get; set; }

        public int ProductType { get; set; }

        public Guid? PrescriptionId { get; set; }
    }
}
