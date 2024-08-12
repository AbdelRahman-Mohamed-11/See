using API.Errors;
using Application.Interfaces;
using Application.pagination;
using Application.Services;
using Core.DTOS;
using Core.DTOS.Email;
using Core.DTOS.filteredDTO;
using Core.DTOS.Identity;
using Core.DTOS.Identity.Managers;
using Core.DTOS.Identity.user;
using Core.DTOS.ProductDTOS;
using Infrastructure.Identity;
using Infrastructure.interfaces;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;
using System.Security.Cryptography;

namespace GlassesApp.Controllers.users
{
    public class ManagersController : BaseApiController
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;
        private readonly ICacheService _redisEmail;
        private readonly IJwtServices _jwtServices;
        private readonly ApplicationDbContext _db;
        private readonly IProductService _productService;
        private readonly ICacheService _inMemoryCache;

        public ManagersController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager, IConfiguration configuration, IEmailService emailService,
            IJwtServices jwtServices, ICacheService redisEmail,
            ApplicationDbContext db, IProductService productService,
            ICacheService inMemoryCache
            )
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._roleManager = roleManager;
            _config = configuration;
            _emailService = emailService;
            this._redisEmail = redisEmail;
            this._jwtServices = jwtServices;
            this._db = db;
            _productService = productService;
            _inMemoryCache = inMemoryCache;
        }


        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> Register(
            ManagerDTO managerDTO)
        {
            // check if the user already exist in database

            var user = await _userManager.FindByEmailAsync(managerDTO.Email);

            if (user is not null)
            {
                if (!user.EmailConfirmed)
                {
                    string codeAgain = GenerateRandomCode();

                    _redisEmail.SetData<string>(managerDTO.Email,
                         codeAgain, DateTime.Now.AddMinutes(10));


                    // send email to the user
                    EmailMessage emailMessageAgain = new EmailMessage
                    {
                        ToAddresses = managerDTO.Email,
                        Subject = "User Confirmation Code",
                        Body = codeAgain
                    };

                    await _emailService.SendEmail(emailMessageAgain);

                    return Ok(new MessageResponse
                    {
                        Message = $"Please Verfiy Your Email Address to enter the code"
                    });
                }

                return BadRequest(new MessageResponse { 
                    Message = $"the email {managerDTO.Email} Already Exist"});
            }

            ApplicationManager manager = new ApplicationManager
            {
                Email = managerDTO.Email,
                UserName = managerDTO.Email,
                FirstName = managerDTO.FirstName,
                LastName = managerDTO.LastName,
                storeName = managerDTO.storeName,
                BusinessLocation = managerDTO.BusinessLocation,
                paymentInfo = managerDTO.PaymentInfo,
                EmailConfirmed = false,
                IsActive = true,
            };


            var managerCreated = await _userManager.CreateAsync(manager, managerDTO.Password);


            if (!managerCreated.Succeeded)
            {
                return BadRequest(new MessageResponse
                {
                    Message = $" Unable to create manager"
                });
    
            }


            if (!await _roleManager.RoleExistsAsync("Manager"))
            {
                var roleResult = await _roleManager.CreateAsync(new ApplicationRole { Name = "Manager" });

                if (!roleResult.Succeeded)
                {
                    return BadRequest(new MessageResponse { Message = $"unable to create manager"});
                }
            }


            //get the manager from database
           var mangerRegistered = await _userManager.FindByEmailAsync(managerDTO.Email);

            // role exist , add the user to this role

            var userToRole = await _userManager.AddToRoleAsync(mangerRegistered!, "Manager");

            if (!userToRole.Succeeded)
            {
                return BadRequest(new MessageResponse
                {
                    Message = $"unable to add role to manager "
                });
            }

            // generate code

            string code = GenerateRandomCode();


            // save code in redis



            _redisEmail.SetData<string>(managerDTO.Email,
             code, DateTime.Now.AddMinutes(3));


            // send email to the user
            EmailMessage emailMessage = new EmailMessage
            {
                ToAddresses = managerDTO.Email,
                Subject = "User Confirmation Code",
                Body = code
            };

            await _emailService.SendEmail(emailMessage);

            return Ok(new MessageResponse
            {
                Message = $"Please Verfiy Your Email Address to enter the code"
            });
        }



        [HttpPost("add-manager")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddManager(
           ManagerDTO managerDTO)
        {
            // check if the user already exist in database

            var user = await _userManager.FindByEmailAsync(managerDTO.Email);

            if (user is not null) 
            {
                return BadRequest(
                    new ApiResponse(400, $"the email {managerDTO.Email} Already Exist"));
            }

            ApplicationManager manager = new ApplicationManager
            {
                Email = managerDTO.Email,
                UserName = managerDTO.Email,
                FirstName = managerDTO.FirstName,
                LastName = managerDTO.LastName,
                storeName = managerDTO.storeName,
                BusinessLocation = managerDTO.BusinessLocation,
                paymentInfo = managerDTO.PaymentInfo,
                EmailConfirmed = false,
                IsActive = true,
            };


            var managerCreated = await _userManager.CreateAsync(manager, managerDTO.Password);


            if (!managerCreated.Succeeded)
            {
                return BadRequest(new ApiResponse(400, $"unable to create the manager"));
            }


            if (!await _roleManager.RoleExistsAsync("Manager"))
            {
                var roleResult = await _roleManager.CreateAsync(new ApplicationRole { Name = "Manager" });

                if (!roleResult.Succeeded)
                {
                    return BadRequest(new ApiResponse(400, $"unable to create the Manager Role"));
                }
            }


            // get the manager from database
            var mangerRegistered = await _userManager.FindByEmailAsync(managerDTO.Email);

            // role exist , add the user to this role

            var userToRole = await _userManager.AddToRoleAsync(mangerRegistered!, "Manager");

            if (!userToRole.Succeeded)
            {
                return BadRequest(new ApiResponse(400, $"unable to add the user to the AppUser Role"));
            }

            // generate code

            string code = GenerateRandomCode();


            // save code in redis

            var userverification = new UserTokenVerificationEmail
            {
                Email = managerDTO.Email,
                VerficationCode = code
            };

            _redisEmail.SetData<UserTokenVerificationEmail>(managerDTO.Email,
               userverification, DateTime.Now.AddMinutes(3));




            // send email to the user
            EmailMessage emailMessage = new EmailMessage
            {
                ToAddresses = managerDTO.Email,
                Subject = "User Confirmation Code",
                Body = code
            };

            await _emailService.SendEmail(emailMessage);

            return Ok("Please Verfiy Your Email Address to enter the code");
        }


        [Authorize(Roles = "Manager,Admin")]
        [HttpGet("accessories")]
        public async Task<ActionResult<PaginatedList<AccessoryDTOResponse>>> GetManagerAccessories(
         [FromQuery] AccessoryFilterDTO accessoryFilterDTO,
    int pageIndex = 1, int pageSize = 10)
        {
            var manager = await 
                _userManager
                .FindByIdAsync(User.FindFirst("ID")?.Value);

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

            var accessories = await _productService.GetManagerAccessoriesAsync(manager.Id,
                accessoryFilterDTO, baseUrl, pageIndex, pageSize);

            return Ok(accessories);
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpGet("glasses")]
        public async Task<ActionResult<PaginatedList<GlassDTOResponse>>> GetManagerGlasses(
    [FromQuery] GlassFilterDTO glassFilterDTO,
    int pageIndex = 1, int pageSize = 10)
        {
            var manager = await _userManager.FindByIdAsync(User.FindFirst("ID")?.Value);

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

            var glasses = await _productService.GetManagerGlassesAsync(
                 manager.Id,
                glassFilterDTO, baseUrl, pageIndex, pageSize);

            return Ok(glasses);
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpGet("lenses")]
        public async Task<ActionResult<PaginatedList<LensesDTOResponse>>> GetManagerLenses(
    [FromQuery] LenseFilterDTO lenseFilterDTO,
    int pageIndex = 1, int pageSize = 10)
        {
            var manager = await _userManager.FindByIdAsync(User.FindFirst("ID")?.Value);

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

            var lenses = await _productService.GetManagerLensesAsync(manager.Id,
                lenseFilterDTO, baseUrl, pageIndex, pageSize);

            return Ok(lenses);
        }



        //[Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ManagerResponse>>> 
            GetManagers()
        {
            var managers = await _db.Managers.ToListAsync();

            var managersResponse = managers
                .Select(m => new ManagerResponse
                {
                    Id = m.Id,
                    BusinessLocation = m.BusinessLocation,
                    Email = m.Email,
                    FirstName = m.FirstName,
                    LastName = m.LastName,
                    storeName = m.storeName,
                    PaymentInfo = m.paymentInfo
                }).ToList();

            return Ok(managersResponse);
        }

        [Authorize(Roles ="Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ManagerResponse>> GetManagerById(Guid id)
        {
            var manager = await _db.Managers.FirstOrDefaultAsync(m => m.Id == id);

            if (manager == null)
                return NotFound("manager not exist , invalid Id");

            var managerResponse = new ManagerResponse
                {
                    Id = manager.Id,
                    BusinessLocation = manager.BusinessLocation,
                    Email = manager.Email,
                    FirstName = manager.FirstName,
                    LastName = manager.LastName,
                    storeName = manager.storeName,
                    PaymentInfo = manager.paymentInfo
                };

            return Ok(managerResponse);
        }


        [HttpPut("update")]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult<bool>> UpdateManager(
            [FromForm] ManagerUpdateDto managerUpdateDto)
        {
            var manager = await _userManager.FindByIdAsync(
                managerUpdateDto.Id?.ToString() ?? User.FindFirst("ID")?.Value) as ApplicationManager;

            if (manager == null)
            {
                throw new Exception("Manager not found");
            }

            if (!String.IsNullOrEmpty(managerUpdateDto.NewEmail) && 
                manager.Email != managerUpdateDto.NewEmail) // Email changed
            {
                manager.Email = managerUpdateDto.NewEmail;

                await _userManager.UpdateAsync(manager);

                return Ok("Manager Email successfully");

            }

            if (!string.IsNullOrEmpty(managerUpdateDto.OldPassword) && 
                !string.IsNullOrEmpty(managerUpdateDto.NewPassword))
            {
                var result = await _userManager.ChangePasswordAsync(manager,
                    managerUpdateDto.OldPassword, managerUpdateDto.NewPassword);

                if (!result.Succeeded)
                {
                    // Handle password change failure
                    throw new Exception("Failed to change password");
                }
            }

            // Update manager's properties if they are provided
            if (!string.IsNullOrEmpty(managerUpdateDto.FirstName))
            {
                manager.FirstName = managerUpdateDto.FirstName;
            }

            if (!string.IsNullOrEmpty(managerUpdateDto.LastName))
            {
                manager.LastName = managerUpdateDto.LastName;
            }

            if (!string.IsNullOrEmpty(managerUpdateDto.storeName))
            {
                manager.storeName = managerUpdateDto.storeName;
            }

            if (!string.IsNullOrEmpty(managerUpdateDto.BusinessLocation))
            {
                manager.BusinessLocation = managerUpdateDto.BusinessLocation;
            }

            await _userManager.UpdateAsync(manager);

            return Ok("Manager updated successfully");
        }



        [HttpGet("activatedManagers")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<ManagerResponse>>> GetManagersActivated()
        {
            var managers = await _db.Managers.Where(m => m.IsActive).ToListAsync();


           var managersResponse = managers
                .Select(m => new ManagerResponse
                {
                    Id = m.Id,
                    BusinessLocation = m.BusinessLocation,
                    Email = m.Email,
                    FirstName = m.FirstName,
                    LastName = m.LastName,
                    storeName = m.storeName,
                    PaymentInfo = m.paymentInfo
                }).ToList();

            return Ok(managersResponse);
        }

        [HttpGet("deactivatedManagers")]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult<List<ManagerResponse>>> GetManagersDeActivated()
        {
            var managers = await _db.Managers.Where(m => !m.IsActive).ToListAsync();


            var managersResponse = managers
                 .Select(m => new ManagerResponse
                 {
                     Id = m.Id,
                     BusinessLocation = m.BusinessLocation,
                     Email = m.Email,
                     FirstName = m.FirstName,
                     LastName = m.LastName,
                     storeName = m.storeName,
                     PaymentInfo = m.paymentInfo
                 }).ToList();

            return Ok(managersResponse);
        }




        [HttpPut("activate")]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult<MessageResponse>> 
            ActivateManager([FromBody] ManagerActivationRequest request)
        {
            // check the user exist 

            var user = await _userManager.FindByIdAsync(request.Id.ToString());

            if (user == null)
            {
                return NotFound(new MessageResponse
                { 
                    Message = "User not found" });
            }

            if (user.IsActive)
            {
               
                return Ok(new MessageResponse
                {
                    Message = "User not found"
                });
            }

            user.IsActive = true;

            var userUpdated = await _userManager.UpdateAsync(user);

            if (!userUpdated.Succeeded)
            {
                return StatusCode(500, new MessageResponse { Message = "Unable to activate the manager" });

            }

            return Ok(new MessageResponse { Message = "The manager has been activated" });
        }



        [HttpPut("deactivate")]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult<MessageResponse>> DeActivateManager([FromBody] ManagerActivationRequest request)
        {
            // check the user exist 

            var user = await _userManager.FindByIdAsync(request.Id.ToString());

            if (user == null)
            {
                return NotFound(new MessageResponse { Message = "the manager is not exist" });
            }

            if (!user.IsActive)
                return Ok(new MessageResponse { 
                    Message = "the manager already De activated" }); ;

            // deactivate the user , and remove the user from role

            user.IsActive = false;

            var userUpdated = await _userManager.UpdateAsync(user); 

            if (!userUpdated.Succeeded)
            {
                return StatusCode(500, 
                    new MessageResponse { Message = "Unable to deactivate the manager" });
            }

            return Ok(new MessageResponse { Message = "manager is De activated successfully"});
        }

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
