using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Order
{
    public enum DeliveryMethod // the base entity contain the ID 
    {
        [EnumMember(Value = "Cash On Delivery")]
        CashOnDelivery,

        [EnumMember(Value = "Credit Card")]
        CreditCard
    }
}
