using Core.Entities;
using Core.Entities.Order;
using Core.Entities.Prescription_Lenses;
using Core.Entities.VisionTest;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ?UserPhotoPath { get; set; } 
        public List<Address> Addresses { get; set; } = new List<Address>();

        public List<PhoneNumber> PhoneNumbers { get; set; } = new List<PhoneNumber>();

        public List<Order> Orders { get; set; } = new List<Order>();

        public List<UserPrescription> Prescriptions { get; set; } = new List<UserPrescription>();

        public List<UserFavoriteProduct> FavoriteProducts { get; set; } = new List<UserFavoriteProduct>();

        public List<VisionTest> VisionTests { get; set; } = new List<VisionTest>(); 

        public string? DeviceId { get; set; }  

        public bool IsActive { get; set; }
        
        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryDate { get; set; }
    }
}
