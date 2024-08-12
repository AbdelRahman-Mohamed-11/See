using Core.DTOS.ProductDTOS;

namespace GlassesApp.Controllers
{
    public class ProductDTO
    {
        public Guid Id { get; set; }
        public int ProductType { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }
        public List<ImageDTOResponse> DefaultPictureUrl { get; set; } = new List<ImageDTOResponse>();
        public List<string> ColorsNames { get; set; }
        public int MostPopular { get; set; }
        public int AvailableQuantity { get; set; }
        public bool IsFavorite { get; set; }
    }
}