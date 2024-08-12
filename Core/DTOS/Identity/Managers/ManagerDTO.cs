using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOS.Identity.Managers
{
    public class ManagerDTO
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]

        public string storeName { get; set; }

        [Required]

        public string BusinessLocation { get; set; }

        [Required]

        public PaymentInfo PaymentInfo { get; set; }
    }
}
