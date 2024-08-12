namespace Application.Interfaces
{
    public class PriceResponse
    {
        public Guid VendorId { get; set; }

        public string vendorName { get; set; }

        public decimal Price { get; set; }
    }
}