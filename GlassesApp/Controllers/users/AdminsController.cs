using API.Errors;
using Core.DTOS.Identity;
using Core.DTOS.Identity.Admins;
using Core.DTOS.Identity.user;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GlassesApp.Controllers.users
{
    //[Authorize(Roles = "Admin")]
    public class AdminsController : BaseApiController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IWebHostEnvironment _env;


        public AdminsController(UserManager<ApplicationUser> userManager , RoleManager<ApplicationRole> roleManager,
            IWebHostEnvironment env)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            _env = env;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApplicationUser>> GetAllAdmins()
        {
            return Ok(await _userManager.GetUsersInRoleAsync("Admin"));
        }

        [HttpPost("add-admin")]
        public async Task<ActionResult<ApplicationUser>> 
            CreateAdmin(AdminDTO adminDTO)
        {
            var user = await _userManager.FindByEmailAsync(adminDTO.Email);

            if (user is not null)
            {
                return BadRequest(new ApiResponse(400, "the user already exist"));
            }

             // check if the role exist or not
            var adminRole = await _roleManager.RoleExistsAsync("Admin");

            if (!adminRole) // the role not exist 
            {
                var roleResult = await _roleManager.CreateAsync(
                    new ApplicationRole { Name = "Admin" });

                if (!roleResult.Succeeded)
                {
                    return BadRequest(new ApiResponse(400, "the role not exist , and problem occurs when add it "));
                }
            }


            ApplicationUser applicationUser = new ApplicationUser
            {
                Email = adminDTO.Email,
                FirstName = adminDTO.FirstName,
                LastName = adminDTO.LastName,
                IsActive = true,
                EmailConfirmed = true,
                UserName = adminDTO.Email,
                PhoneNumber = adminDTO.PhoneNumber,
            };

            var userCeratedResult = await _userManager.CreateAsync(
                applicationUser,adminDTO.Password);

            if (!userCeratedResult.Succeeded)
            {
                return BadRequest(new ApiResponse(400, "unable to create the user "));
            }


            var addedToRole = await _userManager.AddToRoleAsync(applicationUser, "Admin");

            if(!addedToRole.Succeeded)
                return BadRequest(new ApiResponse(400, "unable to add user to the role admin"));

            return Ok(applicationUser);
        }

        [HttpPut("update-admin")]
        public async Task<IActionResult> UpdateAdmin([FromForm]
        UpdateUserDTO updateUserDTO)
        {
            var user = await _userManager
                    .FindByIdAsync(User.FindFirst("ID")?.Value);

            if (user is null)
                throw new Exception("user not found");

            if (user.Email != updateUserDTO.NewEmail)  // Email changed
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

            await _userManager.UpdateAsync(user);

            return Ok("user updated successfully");
        }
    }
}
