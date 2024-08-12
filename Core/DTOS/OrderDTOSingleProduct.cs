using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOS
{
    public class OrderDTOSingleProduct
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }    

        public Guid productId { get; set; }

        public string? street { get; set; }

        public string? newPhoneNumber { get; set; }

        public Guid? ExistingPhoneNumber { get; set; }

        public Guid? addressId { get; set; }

        public Guid? CityId { get; set; }

        public Guid? userPrescriptionId { get; set; }

        public string colorName { get; set; }

        public bool? IsAndroidDevice { get; set; }
    }
}
