using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOS.Identity
{
    public class UserRefreshToken
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}
