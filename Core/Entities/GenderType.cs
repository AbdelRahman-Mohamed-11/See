using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class GenderType : BaseEntity
    {

       public string GenderName { get; set; }

       public List<ProductGenderType> ProductGenderTypes { get; set; }
    }

}
