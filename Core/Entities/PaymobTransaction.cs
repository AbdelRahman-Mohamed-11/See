using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class PaymobTransaction : BaseEntity
    {
        // make relations with order here
        public string TransactionId { get; set; }
        public string OrderPayId { get; set; }
      
        public Guid OrderId { get; set; }
        public Core.Entities.Order.Order Order { get; set; }

        public double Amount { get; set; }
        public string CreatedDate { get; set; }
        public bool IsSucessTransaction { get; set; }
    }
}
