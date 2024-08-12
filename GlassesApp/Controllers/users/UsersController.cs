using API.Errors;
using Application.Interfaces;
using Ardalis.GuardClauses;
using Core.DTOS;
using Core.DTOS.Basket;
using Core.DTOS.Email;
using Core.DTOS.Identity;
using Core.DTOS.Identity.user;
using Core.DTOS.LoginProvider.facebook;
using Core.DTOS.ResetPassword;
using Core.Entities.Delivery;
using Core.Entities.Order;
using Infrastructure.Identity;
using Infrastructure.interfaces;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;

namespace GlassesApp.Controllers.users
{
    public class UsersController : BaseApiController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IConfiguration _config;
        private readonly IJwtServices _jwtServices;
        private readonly IEmailService _emailService;
        private readonly ICacheService _inMemoryCache;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApplicationDbContext _db;


        public UsersController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager, IConfiguration configuration, IEmailService emailService,
            IJwtServices jwtServices, ICacheService redisEmail,
            IWebHostEnvironment env,
            IHttpClientFactory httpContextFactory,
            ApplicationDbContext db)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._roleManager = roleManager;
            _config = configuration;
            _jwtServices = jwtServices;
            _emailService = emailService;
            _inMemoryCache = redisEmail;
            _env = env;
            _httpClientFactory = httpContextFactory;
            _db = db;
        }
        // sign up
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO
            )
        {
            // check if the user already exist in database

            var user = await _userManager.FindByEmailAsync(registerDTO.Email);

            if (user != null)
            {
                if (!user.EmailConfirmed)
                {
                    // If the email exists but hasn't been confirmed, allow re-sending the confirmation email
                    string codeAgain = GenerateRandomCode();
                    _inMemoryCache.SetData<string>(user.Email, codeAgain, 
                        DateTime.Now.AddMinutes(10));

                    EmailMessage emailMessageAgain = new EmailMessage
                    {
                        ToAddresses = user.Email,
                        Subject = "User Confirmation Code",
                        Body = codeAgain
                    };

                    await _emailService.SendEmail(emailMessageAgain);

                    return Ok(new MessageResponse { Message = "Email confirmation code resent. Please check your email and verify your address." });
                }
                else
                {
                    // If the email exists and has been confirmed, return an error
                    return BadRequest(new ApiResponse(400, $"The email {registerDTO.Email} is already registered."));
                }
            }

            ApplicationUser userRegister = new ApplicationUser
            {
                Email = registerDTO.Email,
                UserName = registerDTO.Email,
                FirstName = registerDTO.FirstName,
                LastName = registerDTO.LastName,
                EmailConfirmed = false,
                IsActive = true,
            };

            var userCreated = await _userManager.CreateAsync(
                userRegister, registerDTO.Password);


            if (!userCreated.Succeeded)
            {
                return BadRequest(new ApiResponse(400, $"unable to create the user"));
            }


            if (!await _roleManager.RoleExistsAsync("AppUser"))
            {
                var roleResult = await _roleManager.CreateAsync(new ApplicationRole { Name = "AppUser" });

                if (!roleResult.Succeeded)
                {
                    return BadRequest(new ApiResponse(400, $"unable to create the AppUser Role"));
                }
            }
            // get the user from database
            var userRegistered = await _userManager.FindByEmailAsync(registerDTO.Email);

            // role exist , add the user to this role

            var userToRole = await _userManager.AddToRoleAsync(userRegistered!, "AppUser");

            if (!userToRole.Succeeded)
            {
                return BadRequest(new ApiResponse(400, $"unable to add the user to the AppUser Role"));
            }

            // generate code

            string code = GenerateRandomCode();


            // save code in redis

            _inMemoryCache.SetData<string>(userRegister.Email,
                code, DateTime.Now.AddMinutes(5));




            // send email to the user
            EmailMessage emailMessage = new EmailMessage
            {
                ToAddresses = userRegistered.Email,
                Subject = "User Confirmation Code",
                Body = code
            };

            await _emailService.SendEmail(emailMessage);

            return Ok(new SuccessMessage { Message = "Please Verfiy Your Email Address to enter the code" });
        }


        [HttpPost("send-confirmation-email")]
        [AllowAnonymous]
        public async Task<IActionResult> SendConfirmationEmail(
            EmailDTO emailDto)
        {

            var manager = await _userManager.FindByIdAsync(User.FindFirst("ID")?.Value);

            if (manager == null || manager.Email != emailDto.Email)
            {
                return NotFound("Manager not found , or email incorrect");
            }

            string code = GenerateRandomCode();

            _inMemoryCache.SetData<string>(emailDto.Email, code, DateTime.Now.AddMinutes(5));

            EmailMessage emailMessage = new EmailMessage
            {
                ToAddresses = emailDto.Email,
                Subject = "User Confirmation Code",
                Body = code
            };

            await _emailService.SendEmail(emailMessage);

            return Ok("check Your Email for the code");

        }

        [AllowAnonymous]
        [HttpGet("confirm-email")]
        
        public async Task<IActionResult> EmailConfirmation(
            string email, string code)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                return BadRequest(new ApiResponse(400, "user not found"));
            }

            var verificaionCode = _inMemoryCache.GetData<string>(email);

            if (verificaionCode == code)
            {
                user.EmailConfirmed = true;

                var result = await _userManager.UpdateAsync(user);

                _inMemoryCache.RemoveData(user.Id.ToString());

                return Ok(new MessageResponse { Message = "email confirmed successfully" });

            }
            else
            {
                return BadRequest(new ApiResponse(400, "code is not correct"));
            }

        }

        [HttpPost("assign-user-basket")]
        [Authorize("AppUser")]
        public async Task<IActionResult> AssignUserToBasket(string basketId)
        {
            var user = await _userManager.FindByIdAsync(User.FindFirst("ID")!.Value);

            // check if the user have basket or not 

            var basket = _inMemoryCache.GetData<CustomerBasket>(basketId);

            basket.UserId = user!.Id;

            _inMemoryCache.SetData(basketId, basket, DateTime.Now.AddDays(30));

            return Ok("basket assoicated with user correctly");
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthenticatedResponse>>
            Login(
            LoginDTO loginInformation
            )
        {
            var user = await _userManager.FindByEmailAsync(loginInformation.Email);

            if (user == null)
                return BadRequest(new ApiResponse(400, "invalid login attempt , user email not exist"));

            if (!user.IsActive)
                return BadRequest(new ApiResponse(400, "invalid login attempt , email is not active."));

            if(!user.EmailConfirmed)
            {
                return BadRequest(new ApiResponse(400, "Your email is not confirmed," +
                    " try to register again "));
            }


            // if user exist will try to signin by password 
            var result = await _signInManager.CheckPasswordSignInAsync(
                user, loginInformation.Password, false);

            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponse(400, "invalid login attempt, password is incorrect"));
            }


            // login successfuly




            // GENERATE REFRESH TOKEN FOR THE USER
            var refreshToken = _jwtServices.GenerateRefreshToken();
            var refreshTokenExpireInMinutes = 
                double.Parse(_config["JWT:RefreshTokenExpireInMinutes"]);
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryDate = 
                DateTime.Now.AddMinutes(refreshTokenExpireInMinutes);

            await _userManager.UpdateAsync(user);


            // If we here that means user information is right and we will generate token for him
            var authResponse = 
                await _jwtServices.GenerateJWTokenAsync(user.Id.ToString());

            var roles = await _userManager.GetRolesAsync(user);

            authResponse.RefreshToken = refreshToken;

            authResponse.Roles = roles;

            return authResponse;

        }


        [HttpPost("sendEmailResetPassword")]
        [AllowAnonymous]

        public async Task<IActionResult> SendEmail([FromBody] EmailDTO emailDTO)
        {
            var user = await _userManager.FindByEmailAsync(emailDTO.Email);

            if (user is null || !user.IsActive)
                return NotFound(new ApiResponse(404, "the user not exist"));

            string code = GenerateRandomCode();


            _inMemoryCache.SetData<string>(emailDTO.Email, code, DateTime.Now.AddMinutes(5));


            // send email to the user
            EmailMessage emailMessage = new EmailMessage
            {
                ToAddresses = emailDTO.Email,
                Subject = "User Confirmation Code",
                Body = code
            };

            await _emailService.SendEmail(emailMessage);


            return Ok("Please check the email for the code");

        }


        [HttpPost("resetPassword")]
        [AllowAnonymous]

        public async Task<IActionResult> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            // Retrieve the user from Redis using the provided email
            var userVerification = _inMemoryCache.GetData<string>(resetPasswordDTO.Email);

            if (userVerification == null || userVerification
                != resetPasswordDTO.Code)
            {
                return BadRequest(new ApiResponse(400, "Invalid verification code"));
            }

            // Use the user ID obtained from Redis to find the user
            var user = await _userManager.FindByEmailAsync(resetPasswordDTO.Email);

            if (user == null)
            {
                return NotFound(new ApiResponse(404, "User not found"));
            }

            if (!user.IsActive)
            {
                return BadRequest(new ApiResponse(400, "User is not active. Password reset not allowed."));
            }
            // Reset the user's password
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await
                _userManager.ResetPasswordAsync(user, token, resetPasswordDTO.NewPassword);

            if (!result.Succeeded)
            {
                // invalid token or password requirements not met
                return BadRequest(new ApiResponse(400, "Password reset failed"));
            }

            // Password reset successful, remove the verification code from Redis
            _inMemoryCache.RemoveData(user.Email);

            return Ok("Password reset successful");
        }


        //[Authorize(Roles = "Admin")]
        [HttpGet("getAppUsers")]
        public async Task<IActionResult> GetAppUsers()
        {
            var appUsers = await _userManager.GetUsersInRoleAsync("appUser");
            return Ok(appUsers);
        }

        [Authorize(Roles = "Admin")]

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user is null)
                return NotFound(new ApiResponse(404, "the user is not exist"));

            // delete the user by unactivate it

            user.IsActive = false;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok(new ApiResponse(200, "User deactivated successfully"));
            }
            else
            {
                // Handle the case where updating the user failed
                return StatusCode(500, new ApiResponse(500, "Failed to deactivate the user"));
            }
        }



        [Authorize(Roles = "AppUser")] // here user and admin can update because same properties
        [HttpPut("update-user")]
        public async Task<IActionResult> UpdateUser([FromForm]
        UpdateUserDTO updateUserDTO)
        {
            var user = await _userManager
                    .FindByIdAsync(User.FindFirst("ID")?.Value);

            if (user is null)
                throw new Exception("user not found");

            if (updateUserDTO.NewEmail != null && user.Email != updateUserDTO.NewEmail)  // Email changed
            {
                user.Email = updateUserDTO.NewEmail;

                await _userManager.UpdateAsync(user);

                return Ok("email changed successfully");
            }

            if (!String.IsNullOrEmpty(updateUserDTO.FirstName))
                user.FirstName = updateUserDTO.FirstName;   

            if (!String.IsNullOrEmpty(updateUserDTO.LastName))
                user.LastName = updateUserDTO.LastName;

            if ((!String.IsNullOrEmpty(updateUserDTO.NewPassword) && !String.IsNullOrEmpty(updateUserDTO.OldPassword)))
            {

                var result = await _userManager.ChangePasswordAsync(user, updateUserDTO.OldPassword, updateUserDTO.NewPassword);

                if (!result.Succeeded)
                {
                    // Handle password change failure
                    throw new Exception("Failed to change password");
                }

            }

            if (updateUserDTO.UserPhoto != null)
            {

                var existingPhotoPath = Path.Combine(_env.WebRootPath, "userphotos", user.Id.ToString());
                if (System.IO.Directory.Exists(existingPhotoPath))
                {
                    System.IO.Directory.Delete(existingPhotoPath, true);
                }

                Directory.CreateDirectory(existingPhotoPath);

                // Generate unique file name with GUID
                var uniqueFileName = $"{Guid.NewGuid()}_{updateUserDTO.UserPhoto.FileName}";

                // Convert the uploaded file to a byte array
                byte[] photoBytes;
                using (var memoryStream = new MemoryStream())
                {
                    await updateUserDTO.UserPhoto.CopyToAsync(memoryStream);
                    photoBytes = memoryStream.ToArray();
                }

                // Save the photo to wwwroot or a specific folder with the unique name
                var photoPath = Path.Combine(_env.WebRootPath, "userphotos",
                    user.Id.ToString(), uniqueFileName);

                // Write the byte array to the file
                System.IO.File.WriteAllBytes(photoPath, photoBytes);

                user.UserPhotoPath = uniqueFileName;
            }

            if(updateUserDTO.DeviceID != null)
            {
                user.DeviceId = updateUserDTO.DeviceID;
            }

            if (updateUserDTO.PhoneNumber != null)
            {
                user.PhoneNumber = updateUserDTO.PhoneNumber;
            }

            await _userManager.UpdateAsync(user);

            return Ok("user updated successfully");
        }



        [Authorize]
        [HttpPost("check-old-password")]
        public async Task<IActionResult> CheckOldPassword([FromBody] CheckOldPasswordDTO checkOldPasswordDTO)
        {
            var user = await _userManager.FindByIdAsync(User.FindFirst("ID")?.Value);

            if (user is null)
                return NotFound("User not found");

            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, checkOldPasswordDTO.OldPassword);

            if (!isPasswordCorrect)
                return BadRequest("Incorrect old password provided");

            return Ok("Old password is correct");
        }

        [HttpGet("GetCurrentUser")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var user = await _userManager
                .FindByIdAsync(User.FindFirst("ID")?.Value);
                
            // Get User photo
            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            string ? photoUrl = null;

            if (user.UserPhotoPath != null)
            {
                photoUrl = Path.Combine(baseUrl, "userPhotos", user.Id.ToString(),
                 user.UserPhotoPath);
            }


            var userDtoResponse = new UserDto
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhotoUrl = photoUrl,
                PhoneNumber = user.PhoneNumber
            };

            return Ok(userDtoResponse);

        }

        [HttpGet("GetUserAddresses")]
        [Authorize]
        public async Task<ActionResult<List<Address>>> GetUserAddresses()
        {
            var userId = Guid.Parse(User.FindFirst("ID")?.Value!);

            var addresses = await _db.Address
                            .Where(a => a.UserId == userId)
                            .ToListAsync();
            
            List<AddressDTO> addressesDTO = new List<AddressDTO>();
            
            foreach(var address in addresses) {
                var city = await _db.Cities.FirstOrDefaultAsync(c => c.Id == address.CityId);

                addressesDTO.Add(new AddressDTO
                {
                    AddressId = address.Id,
                    CityId = city.Id,
                    City = city.CityName,
                    Street = address.Street
                });
            
            }


            return Ok(addressesDTO);

        }

        [HttpGet("GetUserPhones")]
        [Authorize]
        public async Task<ActionResult<List<PhoneNumber>>> GetUserPhones()
        {
            var userId = Guid.Parse(User.FindFirst("ID")?.Value!);

            var phones = await _db.PhoneNumbers
                            .Where(p => p.ApplicationUserId == userId)
                            .ToListAsync();

            return Ok(phones);

        }

        [HttpPost("signin-facebook")]
        public async Task<IActionResult> LoginFacebook([FromBody] FacebookTokenDto facebookTokenDto)
        {
            // Verify if the token received is valid using Facebook API
            var facebookUser = await VerifyFacebookTokenAsync(facebookTokenDto.Token);

            if (facebookUser == null)
            {
                return Unauthorized("Invalid Facebook token");
            }

            var loginProvider = "Facebook";
            var providerKey = facebookUser.Id;

            // Sign in a user using external login information
            var result = await _signInManager.ExternalLoginSignInAsync(loginProvider, providerKey, isPersistent: false);

            ApplicationUser user;

            if (!result.Succeeded)
            {
                // Get Facebook user info
                var userInfo = await GetFacebookUserInfoAsync(providerKey, facebookTokenDto.Token);

                // Create a User entity with Facebook information
                user = new ApplicationUser
                {
                    UserName = userInfo.Email,
                    Email = userInfo.Email,
                    FirstName = userInfo.FirstName,
                    LastName = userInfo.LastName,
                    IsActive = true,
                    EmailConfirmed = true
                };

                // Create the user in the database
                var createResult = await _userManager.CreateAsync(user);

                if (!createResult.Succeeded)
                {
                    return BadRequest("Failed to create user");
                }

                // Add Facebook login to the user
                var addLoginResult = await _userManager.AddLoginAsync(user, new UserLoginInfo(loginProvider, providerKey, loginProvider));

                if (!addLoginResult.Succeeded)
                {
                    return BadRequest("Failed to add Facebook login to user");
                }
            }
            else
            {
                // User is already registered, get the user through the id of the provider (Facebook)
                user = await _userManager.FindByLoginAsync(
                    loginProvider, providerKey);

                if (user == null)
                {
                    return BadRequest("Failed to find user by login");
                }
            }

            // Generate a new refresh token
            var refreshToken = _jwtServices.GenerateRefreshToken();
            var refreshTokenExpireInMinutes = double.Parse(_config["JWT:RefreshTokenExpireInMinutes"]);
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryDate = DateTime.Now.AddMinutes(refreshTokenExpireInMinutes);

            // Update the user in the database
            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                return BadRequest("Failed to update user");
            }

            // Generate a new JWT for the user
            var authResponse = await _jwtServices.GenerateJWTokenAsync(user.Id.ToString());
            authResponse.RefreshToken = refreshToken;

            return Ok(authResponse);
        }


        private async Task<FacebookUser?> VerifyFacebookTokenAsync(string token)
        {
            var httpClient = _httpClientFactory.CreateClient();

            var appToken = $"{_config["Facebook:AppId"]}|{_config["Facebook:AppSecret"]}";

            var verificationUrl = $"https://graph.facebook.com/debug_token?input_token={token}&access_token={appToken}";

            var response = await httpClient.GetStringAsync(verificationUrl);

            var result = JsonConvert.DeserializeObject<FacebookVerificationResult>(response);

            if (result.Data?.Is_Valid == true)
            {
                return new FacebookUser { Id = result.Data.User_Id };
            }

            return null;
        }

        private async Task<FacebookUserInfo> GetFacebookUserInfoAsync(string userId, string accessToken)
        {
            var httpClient = _httpClientFactory.CreateClient();

            var userInfoUrl = $"https://graph.facebook.com/{userId}?fields=id,name,email&access_token={accessToken}";

            var response = await httpClient.GetStringAsync(userInfoUrl);

            var userInfo = JsonConvert.DeserializeObject<FacebookUserInfo>(response);

            return userInfo;
        }


        //[HttpGet("access-denied")]
        //public IActionResult AccessDenied()
        //{
        //    return BadRequest("Access Denied: You have denied the required permissions.");
        //}
        private string GenerateRandomCode()
        {
            byte[] randomBytes = new byte[8]; // Use a larger array for a 6-digit code

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            long randomValue = BitConverter.ToInt64(randomBytes, 0);
            long positiveValue = Math.Abs(randomValue);

            return (positiveValue % 900000 + 100000).ToString("D6");
        }



    }
}
