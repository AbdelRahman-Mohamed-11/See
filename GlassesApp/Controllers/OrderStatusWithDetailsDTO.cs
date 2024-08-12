using Core.Entities.Order;

namespace GlassesApp.Controllers
{
    public class OrderStatusWithDetailsDTO
    {
        public Guid OrderId { get; set; }
        public string Status { get; set; }
        public DateTime OrderStartDate { get; set; }
        public DateTime EstimatedDeliveryTime { get; set; }
        public decimal Subtotal { get; set; }
        public Address Address { get; set; }
        public IReadOnlyList<OrderItem> OrderItems { get; set; }
    }
}