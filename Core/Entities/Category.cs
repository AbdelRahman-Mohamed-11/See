namespace Core.Entities
{
    public class Category : BaseEntity
    {

        public string CategoryName { get; set; }
       
        public List<SubCategory> SubCategories { get; set; } // Navigation property
    }
}