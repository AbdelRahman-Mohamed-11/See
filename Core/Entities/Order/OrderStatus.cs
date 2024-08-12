using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Order
{
    public enum OrderStatus
    {
        [EnumMember(Value = "Pending")]
        Pending,          // Order has been placed but not yet processed.

        [EnumMember(Value = "Processing Order for shipment")]
        Processing,       // The order is being prepared for shipment.

        [EnumMember(Value = "Shipped The Order")]
        Shipped,          // The order has been shipped to the customer.

        [EnumMember(Value = "Delivered")]
        Delivered,        // The order has been successfully delivered.

        [EnumMember(Value = "Cancelled")]
        Cancelled,        // The order has been canceled by the customer or the system.

        [EnumMember(Value = "Returned")]
        Returned,         // Items from the order have been returned.

        [EnumMember(Value = "Refunded")]
        Refunded,         // The payment has been refunded.

        [EnumMember(Value = "Payment Failed")]
        PaymentFailed
    }
}
