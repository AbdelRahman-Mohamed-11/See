using Core.DTOS.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.interfaces
{
    public interface IJwtServices
    {
        Task<AuthenticatedResponse> RefreshAsync(UserRefreshToken userRefreshToken);
        Task<AuthenticatedResponse> GenerateJWTokenAsync(string userId);
        string GenerateRefreshToken();
    }
}
