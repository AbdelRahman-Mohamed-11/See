using Core.DTOS.Basket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOS
{
    public class AddToBasketRequestDto
    {
        public string BasketId { get; set; }
        public BasketItem NewItem { get; set; }
    }

}
