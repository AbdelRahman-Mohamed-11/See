using API.Errors;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace GlassesApp.Controllers
{
    
    public class AuthorizationController : BaseApiController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<AuthorizationController> _logger;

        public AuthorizationController(UserManager<ApplicationUser>userManager,
           RoleManager<ApplicationRole> roleManager, ApplicationDbContext dbContext,
           ILogger<AuthorizationController> logger)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            _dbContext = dbContext;
            _logger = logger;
        }
            // roles , claims , policy
        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            return Ok(await _roleManager.Roles.ToListAsync());
        }

        [HttpGet("getUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(await _userManager.Users.ToListAsync());
        }

        [HttpDelete("deleteByEmail")]
        public async Task<IActionResult> DeleteUser([FromBody]string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return NoContent();
            }
            else
            {
                // Handle errors if deletion fails
                return BadRequest(result.Errors);
            }
        }
        
        [HttpGet("getUserRoles")]
        public async Task<IActionResult> GetUserRoles(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null) // the user not exist
            {
                return BadRequest(new ApiResponse(400, "the user is not exist"));
            }

            return Ok(await _userManager.GetRolesAsync(user));
        }

        [HttpPost("addRole")]
        public async Task<IActionResult> AddRole(string name)
        {
            // check the role exist or no 
            if(!await _roleManager.RoleExistsAsync(name)) {
                // add the role
                ApplicationRole applicationRole = new ApplicationRole { Name = name};

                var roleResult = await _roleManager.CreateAsync(applicationRole);

                if (!roleResult.Succeeded) // the rule not addedd
                {
                    _logger.LogInformation($"Something Wrong,the Role {name} Not Added ");
                    
                    return BadRequest(
                        new ApiResponse(400, $"Something Wrong,the Role {name} Not Added "));
                }

                // the role is added successfuly

                return Ok($"the Role {name} is Added succussfuly");
             }



            return BadRequest(new ApiResponse(400, "the role already exist"));
        }

        [HttpPost("addUserToRole")]

        public async Task<IActionResult> addUserToRole(string email , string roleName)
        {
            // 1- check the email exist or no
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null) // the user not exist
            {
                return BadRequest(new ApiResponse(400, "the user is not exist"));
            }

            // 2- check the role exist or no

            var roleExist = await _roleManager.RoleExistsAsync(roleName);

            if(!roleExist) {
                return BadRequest(new ApiResponse(400, "the Role is not exist"));
            }


            // here the role and the user are exist , then should add the user to the role

           var result = await _userManager.AddToRoleAsync(user, roleName);

            if(!result.Succeeded)
            {
                return BadRequest(new ApiResponse(400, "something wrong , the user not added to the Role"));
            }

            return Ok("the user added succsefully to the role");
        }

        [HttpDelete("removeUserFromRole")]
        public async Task<IActionResult> removeUserFromRole(string email, string roleName)
        {
            // 1- check the email exist or no
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null) // the user not exist
            {
                return BadRequest(new ApiResponse(400, "the user is not exist"));
            }

            // 2- check the role exist or no

            var roleExist = await _roleManager.RoleExistsAsync(roleName);

            if (!roleExist)
            {
                return BadRequest(new ApiResponse(400, "the Role is not exist"));
            }

            var removeUser = await _userManager.RemoveFromRoleAsync(user, roleName);

            return removeUser.Succeeded ? Ok($"user {email} remove from role {roleName} successfuly")
                : BadRequest(new ApiResponse(400, "unable to remove user from role "));
        }

    }
}
