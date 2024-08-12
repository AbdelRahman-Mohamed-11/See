namespace GlassesApp.Controllers
{
    public class OrderStatusDTO
    {
        public Guid OrderId { get; set; }
        public string Status { get; set; }
        public DateTime OrderStartDate { get; set; }
        public DateTime EstimatedDeliveryTime { get; set; }
    }

}