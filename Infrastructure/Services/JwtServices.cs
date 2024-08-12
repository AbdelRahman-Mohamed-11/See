using API.Errors;
using Core.DTOS.Identity;
using Infrastructure.Identity;
using Infrastructure.interfaces;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class JwtServices : IJwtServices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IConfiguration _config;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<JwtServices> _logger;
        public JwtServices(SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ILogger<JwtServices> logger,
            IConfiguration config)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
            _signInManager = signInManager;
        }
        public async Task<AuthenticatedResponse> GenerateJWTokenAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            var claims = new List<Claim>
            {
                new Claim("ID", userId),
                new Claim(ClaimTypes.Name, user!.UserName!),
            };

            // get claims assoicated with that user
            
            var userClaims = await _userManager.GetClaimsAsync(user); 


            
            claims.AddRange(userClaims);

            // will assign the claims that associated with the roles
           
            var userRoles = await _userManager.GetRolesAsync(user);  // list of the roles for the user

            foreach (var userRole in userRoles)
            {
                // add the role as claim 
                claims.Add(new Claim(ClaimTypes.Role, userRole));

                // get the identity role for the string role

                var role = await _roleManager.FindByNameAsync(userRole);  

                
                var roleClaims = await _roleManager.GetClaimsAsync(role); // take identity role

                foreach (Claim roleClaim in roleClaims)
                {
                    claims.Add(roleClaim);
                }
            }


            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(_config["JWT:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_config["JWT:TokenExpireInMinutes"])),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"]
            };
            
            var token = tokenHandler.CreateToken(tokenDescriptor);

            _logger.LogInformation($"Generated JWT for user {user.UserName}:");
            _logger.LogInformation($"Token: {tokenHandler.WriteToken(token)}");
            _logger.LogInformation($"Expires at: {tokenDescriptor.Expires}");

            return new AuthenticatedResponse()
            {
                Token = tokenHandler.WriteToken(token),
                UserName = user.UserName,
                ExpireIn = tokenDescriptor.Expires
            };
        }

        
        
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public async Task<AuthenticatedResponse> RefreshAsync(UserRefreshToken userRefreshToken)
        {
            if (userRefreshToken is null)
            {
                throw new Exception("Invalid refresh token details");

            }

            string accessToken = userRefreshToken!.AccessToken!;
            string refreshToken = userRefreshToken.RefreshToken!;

            ClaimsPrincipal? principal = null;

            try
            {
                principal = GetPrincipalFromExpiredToken(accessToken);
            }
            catch (Exception)
            {
                throw new Exception("Invalid access token");

            }

            var username = principal!.Identity!.Name; //this is mapped to the Name claim by default


            ApplicationUser? user = null;

            if (username != null) user = await _userManager.FindByNameAsync(username);

            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryDate <= DateTime.Now)
            {
                throw new Exception("Invalid client request");
            }

            var newAccessToken = await GenerateJWTokenAsync(user.Id.ToString());
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await _userManager.UpdateAsync(user);


            _logger.LogInformation($"User {username} successfully refreshed access token.");


            return new AuthenticatedResponse()
            {
                Token = newAccessToken.Token,
                RefreshToken = newRefreshToken,
                UserName = username,
                ExpireIn = newAccessToken.ExpireIn
            };
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            var key = Encoding.UTF8.GetBytes(_config["JWT:Key"]);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _config["JWT:Issuer"],
                ValidAudience = _config["JWT:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key)

            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters,
                out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;

        }
    }

}
