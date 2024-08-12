using Core.Entities;


namespace Core.DTOS.ProductDTOS
{
    public class LensesDTOResponse : BaseEntity
    {
        public string ProductName { get; set; }

        public Guid ManagerId { get; set; }

        public string Description { get; set; }

        public List<ImageDTOResponse> PictureUrl { get; set; }

        public decimal Price { get; set; }

        public List<string> ColorsNames { get; set; }

        public List<string> Genders { get; set; }


        public string Category { get; set; }


        public string Brand { get; set; }


        public double LensBaseCurve { get; set; }

        public double Lensdiameter { get; set; }

        public string LensUsage { get; set; }


        public double WaterContent { get; set; }

        public int ProductType { get; set; }

        public int AvailableQuantity { get; set; }

        public int MostPopular { get; set; }

        public bool IsFavorite { get; set; }
    }
}
