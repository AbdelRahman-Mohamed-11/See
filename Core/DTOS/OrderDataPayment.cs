using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOS
{
    public class OrderDataPayment
    {
        public bool DeliveryNeeded { get; set; }
        
        public string AmountCents { get; set; }
        
        public string Currency { get; set; }
        
        public int MerchantOrderId { get; set; }

        public List<Item> Items { get; set; }

        public List<ShippingData> ShippingData { get; set; }

        public List<ShippingDetails> ShipingDetails { get; set; }
    }
}
