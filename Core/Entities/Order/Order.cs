using Core.DTOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Order
{
    public class Order : BaseEntity
    {
        public Order() { }
        public Order(IReadOnlyList<OrderItem> orderItems,
            Address shipToAddress,
            decimal subTotal , Guid userId,
            string PhoneNumber)
        {
            ShipToAddress = shipToAddress;
            OrderItems = orderItems;
            SubTotal = subTotal;
            UserId = userId;
            this.PhoneNumber = PhoneNumber;
        }

        public Guid UserId { get; set; } // the id of the user that place the orde
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public Address ShipToAddress { get; set; }

        public IReadOnlyList<OrderItem> OrderItems { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public decimal SubTotal { get; set; }

        public string PhoneNumber { get; set; }

        public string? OrderPayId { get; set; }  // orderId in paymob

        public string? TransactionId { get; set; } // transationId in paymob

        public DateTime EstimatedDeliveryTime { get; set; } // This is the EstimatedDeliveryTime property


    }

}
