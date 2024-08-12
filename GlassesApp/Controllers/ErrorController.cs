using API.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlassesApp.Controllers
{
    [Route("error/{code}")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [AllowAnonymous]
    public class ErrorController : BaseApiController
    {
        [HttpGet]
        public IActionResult Error(int code)  // will get it from the placeholder in the middlware
        {
            return new ObjectResult(new ApiResponse(code));
        }
    }
}
