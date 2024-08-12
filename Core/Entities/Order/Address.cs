using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Order
{
    public class Address : BaseEntity
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }   
        
        public Guid CityId { get; set; } // string

        public string Street { get; set; }

        public Guid UserId { get; set; }
    }
}
