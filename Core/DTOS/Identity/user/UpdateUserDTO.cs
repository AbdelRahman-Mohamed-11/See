using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOS.Identity.user
{
    public class UpdateUserDTO
    {
        public string? DeviceID { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? OldPassword { get; set; }

        public string? NewPassword { get; set; }

        public string? NewEmail { get; set; }

        public IFormFile? UserPhoto { get; set; }

        public string ? PhoneNumber { get; set; }
    }
}
