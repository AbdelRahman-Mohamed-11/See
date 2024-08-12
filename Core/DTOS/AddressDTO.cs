using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOS
{
    public class AddressDTO
    {
        public Guid AddressId { get; set; }
        
        public Guid CityId { get; set; }
        public string City {  get; set; }

        public string Street { get; set; }
    }
}
