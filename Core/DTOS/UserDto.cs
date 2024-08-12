using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOS
{
    public class UserDto
    {
        public string Email { get; set; }

        public string FirstName { get; set; }
       
        public string LastName { get; set; }

        public string PhotoUrl { get; set; }

        public string PhoneNumber { get; set; }

    }
}
