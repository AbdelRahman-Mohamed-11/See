using Core.DTOS;
using Core.Entities.VisionTest;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GlassesApp.Controllers
{
    [Authorize]
    public class VisionTestController : BaseApiController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;
        public VisionTestController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext db) {
            _userManager = userManager;
            _db = db;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<VisionTestResponse>>
            UserVisionTests()
        {
            var userId = User.FindFirst("ID")?.Value;

            var user = await _userManager.Users
            .Include(u => u.VisionTests)
            .FirstOrDefaultAsync(u => u.Id.ToString() == userId);


            var visionTests = user?.VisionTests
                .Select(
                 v => new VisionTestResponse
                 {
                     VisionTestDate = v.VisionTestDate,
                     LeftEyeResult = v.LeftEyeResult,
                     RightEyeResult = v.RightEyeResult,
                     Score = v.Score,
                     Id = v.Id
                 })
                .OrderBy(v => v.VisionTestDate).ToList();


            return Ok(visionTests);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<VisionTestResponse>> GetVisionTest(Guid id)
        {
           var visionTest = await _db.VisionTests.FirstOrDefaultAsync(v => v.Id == id);
            
           if (visionTest == null)
            {
                return NotFound("the vision test ID not valid");
            }

           return Ok(new VisionTestResponse { Id = visionTest.Id,
           Score = visionTest.Score,
           LeftEyeResult = visionTest.LeftEyeResult,
           RightEyeResult = visionTest.RightEyeResult,
           VisionTestDate = visionTest.VisionTestDate});

        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<bool>> AddVisionTestScore(VisionTestAddDTO visionTestAddDTO)
        {
            var user = await _userManager
                   .FindByIdAsync(User.FindFirst("ID")?.Value);

            await _db.VisionTests.AddAsync(
                new VisionTest
            {
                ApplicationUserId = user.Id,
                LeftEyeResult = visionTestAddDTO.LeftEyeResult,
                VisionTestDate = visionTestAddDTO.VisionTestDate,
                Score = visionTestAddDTO.Score,
                RightEyeResult = visionTestAddDTO.RightEyeResult
                
            });

            await _db.SaveChangesAsync();

            return Ok(true);
        }

        
    }
}
