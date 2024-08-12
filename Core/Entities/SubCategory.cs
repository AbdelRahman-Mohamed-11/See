using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class SubCategory : BaseEntity
    {
        public string SubCategoryName { get; set; }
        public Guid CategoryId { get; set; } // Foreign key to Category
        public Category Category { get; set; } // Navigation property
    }
}
