using Core.Entities;

namespace Core.DTOS.ProductDTOS
{
    public class AccessoryDTOResponse : BaseEntity
    {
        public string ProductName { get; set; }

        public string Description { get; set; }

        // QuantityAvailable

        public List<ImageDTOResponse> PictureUrl { get; set; }

        public List<string> ColorsNames { get; set; }


        public decimal Price { get; set; }

        public List<string> Genders { get; set; }

        public string Category { get; set; }

        public string Brand { get; set; }

        public int ProductType { get; set; }

        public int AvailableQuantity { get; set; }

        public int MostPopular { get; set; }

        public bool IsFavorite { get; set; }

        public Guid ManagerId { get; set; }

        // should return also quantity and popular
    }
}
