using API.Errors;
using Application.Interfaces;
using Application.pagination;
using Core.DTOS;
using Core.DTOS.filteredDTO;
using Core.DTOS.ProductDTOS;
using Core.DTOS.ProductDTOS.UpdateDTOS;
using Core.Entities;
using Core.Products;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Python.Runtime;
using System.Diagnostics;
using System.Text;
using Tensorflow;
using Tensorflow.Operations.Activation;


namespace GlassesApp.Controllers
{

    public class ProductsController : BaseApiController
    {
        private readonly IProductService _productService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;

        public ProductsController(IProductService productService,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext db)
        {
            _productService = productService;
            _context = context;
            _userManager = userManager;
            _db = db;
        }
        [HttpPost("create-shhhhhhhhhhhhape")]
        public async Task<ActionResult<bool>> AddShape(ShapeDTO shapeDTO)
        {
            Core.Entities.Shape shape = new Core.Entities.Shape
            {
                ShapeName = shapeDTO.Name,
            };

            await _db.Shapes.AddAsync(shape);

            await _db.SaveChangesAsync();

            return true;
        }

        // Get EndPoints 
        [HttpGet("search")]
        public async Task<ActionResult<PaginatedList<BaseEntity>>>
            Search(string? search,
            int pageIndex = 1, int pageSize = 10)
        {
            var userId = User?.FindFirst("ID")?.Value;

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

            var products = await _productService
                .Search(baseUrl, search, userId == null ? 
                    null : Guid.Parse(userId), pageSize);

            if (products is null)
                return NotFound("there are no products matched");

            return Ok(products);

        }

        [AllowAnonymous]
        [HttpPost("accessories")]
        public async Task<ActionResult<PaginatedList<AccessoryDTOResponse>>>
            GetAccessories([FromBody] AccessoryFilterDTO accessoryFilterDTO,
            int pageIndex = 1, int pageSize = 10)
        {
            var user = await _userManager.FindByIdAsync(User.FindFirst("ID")?.Value);

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";


            var products = await _productService
                .GetAccessoriesAsync(
                accessoryFilterDTO, baseUrl,
                accessoryFilterDTO.Search,
                user == null ? null : user.Id, pageIndex, pageSize);


            return Ok(products);
        }
        [AllowAnonymous]
        [HttpPost("glasses")]
        public async Task<ActionResult<PaginatedList<GlassDTOResponse>>>
            GetGlasses(
            [FromBody] GlassFilterDTO glassFilterDTO, [FromQuery] 
             int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var user = await _userManager
                .FindByIdAsync(User.FindFirst("ID")?.Value);

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

            var products = await _productService.GetGlassesAsync(
                glassFilterDTO,
                baseUrl, glassFilterDTO.Search,
                user == null ? null : user.Id, pageIndex, pageSize);


            return Ok(products);
        }
        [AllowAnonymous]
        [HttpPost("lenses")]
        public async Task<ActionResult<PaginatedList<
            LensesDTOResponse>>>
            GetLenses([FromBody] LenseFilterDTO lenseFilterDTO,
            int pageindex = 1, int paageSize = 10)
        {
            var user = await _userManager.FindByIdAsync(
                User.FindFirst("ID")?.Value
                );


            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            var products = 
                await _productService
                .GetLensesAsync(lenseFilterDTO,
                baseUrl, lenseFilterDTO.Search,
                user == null ? null : user.Id, pageindex, paageSize);

            // foreach (var product in products.Data)
            // {
            //     product.PictureUrl = Path.Combine(baseUrl, product.PictureUrl);
            // }

            return Ok(products);
        }


        [AllowAnonymous]
        [HttpGet("accessories-mobile")]
        public async Task<ActionResult<PaginatedList<AccessoryDTOResponse>>>
            GetAccessoriesMobile([FromQuery] AccessoryFilterDTO accessoryFilterDTO,
            int pageIndex = 1, int pageSize = 10)
        {
            var user = await _userManager.FindByIdAsync(User.FindFirst("ID")?.Value);

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";


            var products = await _productService
                .GetAccessoriesAsync(
                accessoryFilterDTO, baseUrl,
                accessoryFilterDTO.Search,
                user == null ? null : user.Id, pageIndex, pageSize);


            return Ok(products);
        }
        [AllowAnonymous]
        [HttpGet("glasses-mobile")]
        public async Task<ActionResult<PaginatedList<GlassDTOResponse>>>
            GetGlassesMobile(
            [FromQuery] GlassFilterDTO glassFilterDTO, [FromQuery]
             int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var user = await _userManager
                .FindByIdAsync(User.FindFirst("ID")?.Value);

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

            var products = await _productService.GetGlassesAsync(
                glassFilterDTO,
                baseUrl, glassFilterDTO.Search,
                user == null ? null : user.Id, pageIndex, pageSize);


            return Ok(products);
        }
        [AllowAnonymous]
        [HttpGet("lenses-mobile")]
        public async Task<ActionResult<PaginatedList<
            LensesDTOResponse>>>
            GetLensesMobile([FromQuery] LenseFilterDTO lenseFilterDTO,
            int pageindex = 1, int paageSize = 10)
        {
            var user = await _userManager.FindByIdAsync(
                User.FindFirst("ID")?.Value
                );


            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            var products =
                await _productService
                .GetLensesAsync(lenseFilterDTO,
                baseUrl, lenseFilterDTO.Search,
                user == null ? null : user.Id, pageindex, paageSize);

            // foreach (var product in products.Data)
            // {
            //     product.PictureUrl = Path.Combine(baseUrl, product.PictureUrl);
            // }

            return Ok(products);
        }


        [AllowAnonymous]
        [HttpGet("glass/{id}")]
        public async Task<ActionResult<GlassDTOResponse>> GetGlass(
            Guid id)
        {

            var user = await _userManager
               .FindByIdAsync(User.FindFirst("ID")?.Value);


            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

            var product = await _productService.GetProductByIdAsync
                (id, baseUrl, user == null ? null : user.Id);

            var glass = product as GlassDTOResponse;

            if (glass == null)
            {
                return NotFound(new ApiResponse(404, "The Type of the product is UnKnown"));
            }

            // glass.PictureUrl = Path.Combine(baseUrl, glass.PictureUrl);

            return Ok(glass);
        }

        [AllowAnonymous]
        [HttpGet("accessory/{id}")]
        public async Task<ActionResult<AccessoryDTOResponse>> GetAccessory(Guid id)
        {
            var user = await _userManager
              .FindByIdAsync(User.FindFirst("ID")?.Value);


            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

            var product = await _productService.GetProductByIdAsync(
                id, baseUrl , user == null ? null : user.Id);
            var accessory = product as AccessoryDTOResponse;

            if (accessory == null)
            {
                return NotFound(new ApiResponse(404, "The Type of the product is UnKnown"));
            }


            //var pathUrl = Path.Combine(baseUrl, "managersProductsPhotos",
            //     user.Id.ToString(),
            //    product.Id.ToString());

            //product = product.PictureUrl.Select(i =>
            //Path.Combine(pathUrl, i)).ToList();

            // accessory.PictureUrl = Path.Combine(baseUrl, accessory.PictureUrl);

            return Ok(accessory);
        }

        [AllowAnonymous]
        [HttpGet("lense/{id}")]
        public async Task<ActionResult<LensesDTOResponse>> 
            GetLense(Guid id)
        {
            var user = await _userManager
             .FindByIdAsync(User.FindFirst("ID")?.Value);

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
           
            var product = await _productService.GetProductByIdAsync(
                id, baseUrl, user == null ? null : user.Id);
            var lenses = product as LensesDTOResponse;

            if (lenses == null)
            {
                return NotFound(new ApiResponse(404, "The Type of the product is UnKnown"));
            }

            // lenses.PictureUrl = Path.Combine(baseUrl, lenses.PictureUrl);

            return Ok(lenses);

        }


        //[Authorize(Roles ="Manager")]
        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<Brand>>> GetBrands()
        {
            return await _db.Brands.ToListAsync();
        }

      

        [HttpGet("get-subcategories")]
        public async Task<ActionResult<IReadOnlyList<SubCategory>>>
            GetSubcategories()
        {
            return await _db.SubCategories.ToListAsync();
        }

        [HttpGet("get-colors")]

        public async Task<ActionResult<IReadOnlyList<Color>>>
            GetColors()
        {
            return await _db.Colors.ToListAsync();
        }



        [HttpGet("get-shapes")]

        public async Task<ActionResult<IReadOnlyList<Core.Entities.Shape>>>
            GetShapesType()
        {
            return await _db.Shapes.ToListAsync();
        }

        [HttpGet("get-frames")]

        public async Task<ActionResult<IReadOnlyList<Core.Entities.FrameType>>>
            GetFrames()
        {
            return await _db.FrameTypes.ToListAsync();
        }

        [HttpGet("genders")]

        public async Task<ActionResult<IReadOnlyList<GenderType>>>
            GetGenders()
        {
            return await _db.GenderTypes.ToListAsync();
        }

        // Post EndPoints 

        [HttpPost("create-accessory")]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<Guid?>> CreateAccessory(
            [FromForm] AccessoryDtoRequest productRequest)
        {
            var user = await _userManager.FindByIdAsync(User.FindFirst("ID")?.Value);


            Guid? productId = await
                _productService.CreateAccessoryAsync(productRequest, user.Id);


            if (productId == null)
            {
                return BadRequest(new ApiResponse(400));
            }

            return Ok(productId);
        }

        [HttpPost("create-glass")]
        //[Authorize(Roles = "Manager")]
        public async Task<ActionResult<Guid?>> CreateGlass([FromForm]
        GlassDtoRequest productRequest)
        {
            var user = await _userManager
                .FindByIdAsync(User.FindFirst("ID")?.Value);


            Guid? productId = await _productService
                .CreateGlassAsync(productRequest, user.Id);


            if (productId == null)
            {
                return BadRequest(new ApiResponse(400));
            }


            return Ok(productId);
        }

        [HttpPost("create-lense")]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<Guid>> CreateLense([FromForm] LensDtoRequest productRequest)
        {
            var user = await _userManager.FindByIdAsync(User.FindFirst("ID")?.Value);

            Guid? productId = await _productService
                .CreateLenseAsync(productRequest, user.Id);


            if (productId == null)
            {
                return BadRequest(new ApiResponse(400));
            }

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";


            return Ok(productId);
        }

        [HttpPost("add-brand")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> CreateBrand([FromBody] string brandName)
        {
            var brand = await _productService.AddBrand(brandName);

            if (brand == null) return BadRequest(new ApiResponse(400, "Creation Not complieted !!"));

            return Ok(brand);
        }

       


        // Put EndPoints 
        [Authorize(Roles = "Manager")]
        [HttpPut("accessory")]

        public async Task<ActionResult<bool>> UpdateAccessory(
            [FromForm] AccessoryUpdateDTO accessoryUpdate)
        {
            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

            var IsUpdated = await _productService.UpdateAccessory(accessoryUpdate, baseUrl);

            if (IsUpdated == false)
            {
                return BadRequest(new ApiResponse(400));
            }


            return Ok(IsUpdated);
        }

        //[Authorize(Roles = "Manager")]
        [HttpPut("glass")]
        public async Task<ActionResult<bool>> UpdateGlass(
          [FromForm] GlassUpdateDTO glassUpdate)
        {
            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

            var IsUpdated = await _productService.UpdateGlass(glassUpdate, baseUrl);

            if (!IsUpdated)
            {
                return BadRequest(new ApiResponse(400));
            }


            return Ok(IsUpdated);
        }

        [Authorize(Roles = "Manager")]
        [HttpPut("lense")]
        public async Task<ActionResult<bool>> UpdateLense([FromForm] LenseUpdateDTO lenseUpdate)
        {
            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

            var IsUpdated = await _productService.UpdateLense(lenseUpdate, baseUrl);

            if (!IsUpdated)
            {
                return BadRequest(new ApiResponse(400));
            }


            return Ok(IsUpdated);
        }

        // Delete EndPoints 
        [Authorize(Roles = "Manager")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteProduct(Guid id)
        {
            var Isdeleted = await _productService.DeleteProductAsync(id);

            if (!Isdeleted)
                return BadRequest(new ApiResponse(400, "the product is not exist"));

            return Ok("the product is successfuly deleted");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("brands/{id}")]
        public async Task<ActionResult<bool>> DeleteBrand(Guid id)
        {
            var brand = await _productService.DeleteBrandById(id);
            if (brand == false) return NotFound(new ApiResponse(404, "Deleteing Not Completed !!"));

            return Ok("Brand deleted Completed");
        }

        [HttpGet("get-categories")]
        public async Task<ActionResult<Category>> GetCategories()
        {
            var categories = await _db
                .Categories
                .Include(c => c.SubCategories)
                .ToListAsync ();

            return Ok(categories);
        }



        [HttpDelete("categories/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> Deletecategory(Guid id)
        {
            var category = await _productService.DeleteCategoryById(id);

            if (category == false) return NotFound(new ApiResponse(404, "Deleteing Not Completed !!"));

            return Ok("category Deleted Completed");
        }

        // Temp Endpoint To Add Gender 
        [HttpPost("add-gender")]
        public async Task<ActionResult<GenderType>> CreateGender([FromBody] string genderName)
        {
            var newGender = new GenderType
            {
                GenderName = genderName,
            };
            var result = await _context.GenderTypes.AddAsync(newGender);

            await _context.SaveChangesAsync();
            return Ok(newGender);
        }

        // Color endPoints 
        [HttpPost("create-color")]
        public async Task<ActionResult<Color>> CreateColor([FromBody] string color)
        {
            Color colorCreated = await _productService.CreateColor(color);

            return Ok(colorCreated);
        }



        // Reviews 



    }
}
