using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOS
{
    public class OrderRequestModel
    {
        public OrderRequestModel(int intgeratioId) { 
           
            this.integration_id = intgeratioId;
               
        }
        public string auth_token { get; set; }
        public string amount_cents { get; set; }
        public int expiration { get; set; }
        public string order_id { get; set; }
        public BillingData billing_data { get; set; }
        public string currency { get; set; }
        public int? integration_id { get; set; }
        public string lock_order_when_paid { get; set; }
    }
}
