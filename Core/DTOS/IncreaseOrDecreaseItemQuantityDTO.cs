using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOS
{
    public class IncreaseOrDecreaseItemQuantityDTO
    {
        public string BasketId { get; set; } 
        public string ItemId { get; set; }

        public bool Increased { get; set; } = true;
    }
}
