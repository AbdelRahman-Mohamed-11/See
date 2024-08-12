using Core.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Product : BaseEntity
    {
        public string ProductName { get; set; }
        public string Description { get; set; }

        public int AvailableQuantity { get; set; }   

        public int MostPopular { get; set; }
        //public string? PictureUrl { get; set; }

        public List<Image> PicturesUrl { get; set; } = new List<Image>();

        // public string? Color { get; set; }

        public decimal Price { get; set; }

        public decimal DiscountedPrice { get; set; }


        //public GenderType GenderType { get; set; }
        //[JsonIgnore]
        public List<ProductGenderType> ProductGenderTypes { get; set; } = new List<ProductGenderType>();
        // category

        public List<ProductColor> ProductColors { get; set; } = new List<ProductColor>();

        public List<UserFavoriteProduct> FavoriteProducts { get; set; } = new List<UserFavoriteProduct>();

        public Category Category { get; set; } 

        public Guid CategoryId { get; set; }

        public Brand Brand { get; set; }

        public Guid BrandId { get; set; }

        // the manager who own the product

        public Guid ManagerId { get; set; }

        // relationship with review
        public List<Review> ? Reviews { get; set; }

    }

    
}
