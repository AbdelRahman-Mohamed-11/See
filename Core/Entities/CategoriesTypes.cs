using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class CategoriesTypes : BaseEntity
    {
        public string typeName { get; set; }

        public List<Category> categories { get; set; }
    }
}
