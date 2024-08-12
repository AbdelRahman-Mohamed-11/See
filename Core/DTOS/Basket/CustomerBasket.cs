using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOS.Basket
{
    public class CustomerBasket
    {
        public CustomerBasket() { }
        public CustomerBasket(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }  // generated id 

        public Guid? UserId { get; set; }
        
        public List<BasketItem> Items { get; set; } = new List<BasketItem>();
    }
}