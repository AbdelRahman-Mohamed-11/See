using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOS.Email
{
    public class UserTokenVerificationEmail
    {   
            public UserTokenVerificationEmail() { }
            public UserTokenVerificationEmail(string email)
            {
                Email = email;
            }

            public string Email { get; set; } 

            public string VerficationCode { get; set; }
        
    }
}
