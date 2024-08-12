using Application.Interfaces;
using Application.pagination;
using Core.DTOS.ProductDTOS;
using Core.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using OneOf.Types;
using Python.Runtime;
using System;
using System.IO; // Added for FileStream
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GlassesApp.Controllers
{

    public class ModelResponseDTO
    {
        public string Message { get; set; }
        public string ImageBase64 { get; set; }
    }
    public class ModelRepsonseDTOVideo
    {
        public string Message { get; set; }
        public string VideoUrl { get; set; }
    }
    public class ModelsController : BaseApiController
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _host;
        private readonly HttpClient _httpClient;
        private readonly IProductService _productService;
        public ModelsController(
            ApplicationDbContext db, 
            IWebHostEnvironment host, HttpClient httpClient,
            IProductService productService)
        {
            _db = db;
            _host = host;
            _httpClient = httpClient;
            _productService = productService;
        }


        

        [HttpPost("apply-glass")]
        [Authorize]
        public async Task<ActionResult<ModelResponseDTO>> ApplySunglasses(
            [FromForm] GlassesRequest request)
        {
            var glassImage = await _db.Images
                .Include(i => i.Product)
                .FirstOrDefaultAsync(i => i.Id == request.GlassImageId);

            if (glassImage == null)
                return BadRequest("The imageId is incorrect. Image cannot be null.");

            string imagePath = Path.Combine(_host.WebRootPath,
                "managersProductsPhotos",
                glassImage.Product.ManagerId.ToString(),
                glassImage.ProductId.ToString(), glassImage.Name);

            // Your existing code to retrieve image bytes
            byte[] faceImageBytes;
            using (var memoryStream = new MemoryStream())
            {
                await request.FaceImage.CopyToAsync(memoryStream);
                faceImageBytes = memoryStream.ToArray();
            }

            // New code using FileStream to read image bytes
            byte[] sunglassesImageBytes;
            using (var fileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
            {
                sunglassesImageBytes = new byte[fileStream.Length];
                fileStream.Read(sunglassesImageBytes, 0, (int)fileStream.Length);
            }

            // Prepare data to send to Flask server
            var data = new
            {
                face_image_bytes = Convert.ToBase64String(faceImageBytes),
                sunglasses_image_bytes = Convert.ToBase64String(sunglassesImageBytes)
            };

            // Serialize data to JSON
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);

            using (var client = new HttpClient())
            {
                var content = new StringContent(json,
                    Encoding.UTF8, "application/json");

                var response = await client
                    .PostAsync("https://7f21-196-157-38-166.ngrok-free.app/glass/apply-glass",
                    content);

                if (response.IsSuccessStatusCode)
                {
                    var contentStream = await response.Content.ReadAsStreamAsync();
                    byte[] responseBytes;
                    using (var memoryStream = new MemoryStream())
                    {
                        await contentStream.CopyToAsync(memoryStream);
                        responseBytes = memoryStream.ToArray();
                    }
                    var imageBase64 = Convert.ToBase64String(responseBytes);

                    return Ok(new ModelResponseDTO
                    {
                        Message = "Success",
                        ImageBase64 = imageBase64
                    });
                }
                else
                {
                    return BadRequest("Failed to apply glasses.");
                }
            }
        }


        [HttpPost("face-detection")]
        [Authorize]
        public async Task<ActionResult<PaginatedList<GlassDTOResponse>>> 
            FaceDetection([FromForm] ImageDetectionDto imageDetectionDto)
        {
            if (imageDetectionDto.Image == null || 
                imageDetectionDto.Image.Length == 0)
            {
                return BadRequest("No image uploaded.");
            }

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await imageDetectionDto.Image.CopyToAsync(memoryStream);
                    var byteArrayContent = new ByteArrayContent(memoryStream.ToArray());
                    byteArrayContent.Headers.ContentType = MediaTypeHeaderValue.Parse(
                        imageDetectionDto.Image.ContentType);

                    using (var formData = new MultipartFormDataContent())
                    {
                        formData.Add(byteArrayContent, "image", imageDetectionDto.Image.FileName);

                        var response = await _httpClient.PostAsync("https://8301-196-157-37-236.ngrok-free.app/predict", formData);

                        if (response.IsSuccessStatusCode)
                        {
                            Shape s1, s2;
                            var jsonResponse = await response.Content.ReadAsStringAsync();
                            
                            var result = JsonConvert.DeserializeObject<PredictionResponse>(jsonResponse);

                            if (result == null)
                                return BadRequest("object cannot desrialized");

                            if(result.PredictedClass == "Heart")
                            {
                                 s1 = await _db.Shapes.FirstOrDefaultAsync(s => s.ShapeName == "Cat-eye");
                                 s2 = await _db.Shapes.FirstOrDefaultAsync(s => s.ShapeName == "Browline");

                            }

                            else if(result.PredictedClass == "Oblong")
                            {
                                 s1 = await _db
                                    .Shapes.FirstOrDefaultAsync(s => s.ShapeName == "Round");
                                 s2 = await _db.Shapes.FirstOrDefaultAsync(s => s.ShapeName == "Aviator");
                            }

                            else if (result.PredictedClass == "Round")
                            {
                                 s1 = await _db
                                    .Shapes.FirstOrDefaultAsync(s => s.ShapeName == "Rectangle");
                                 s2 = await _db.Shapes.FirstOrDefaultAsync(s => s.ShapeName == "Square");
                            }

                            else if (result.PredictedClass == "Oval")
                            {
                                 s1 = await _db
                                   .Shapes.FirstOrDefaultAsync(s => s.ShapeName == "Geometric");
                                 s2 = await _db.Shapes.FirstOrDefaultAsync(s => s.ShapeName == "Aviator");
                            }

                            else if(result.PredictedClass == "Square")
                            {
                                 s1 = await _db
                                   .Shapes.FirstOrDefaultAsync(s => s.ShapeName == "Round");
                                 s2 = await _db.Shapes.FirstOrDefaultAsync(s => s.ShapeName == "Oval");
                            }

                            else
                            {
                                return BadRequest("model return a unexpected response");
                            }

                            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

                            var userId = User?.FindFirst("ID")?.Value;

                            var responseDTO =
                                await _productService.GetGlassesAsync(
                                    new Core.DTOS.filteredDTO.GlassFilterDTO
                                    {
                                        ShapeIds = new List<Guid>() { s1.Id },

                                    },

                                    baseUrl,

                                    null,

                                    Guid.Parse(userId)
                                  );


                            return Ok(responseDTO);
                        }
                        else
                        {
                            return StatusCode((int)response.StatusCode, response.ReasonPhrase);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

            

        }

        [HttpPost("apply-video")]
        [Authorize]
        public async Task<ActionResult> ApplyGlassesToVideo([FromForm] VideoModelDTO videoModelDTO)
        {
            if (videoModelDTO.videoFile.Length == 0 || videoModelDTO.ImageId == Guid.Empty)
            {
                return BadRequest("Invalid video file or ImageId.");
            }

            var userId = User?.FindFirst("ID")?.Value;
            if (userId == null)
            {
                return Unauthorized("User ID is required.");
            }

            try
            {
                // Retrieve the image from the database based on ImageId
                var glassImage = await _db.Images
                    .Include(i => i.Product)
                    .FirstOrDefaultAsync(i => i.Id == videoModelDTO.ImageId);

                if (glassImage == null)
                {
                    return BadRequest("Image with the provided ImageId not found.");
                }

                // Path to the image file
                string imagePath = Path.Combine(_host.WebRootPath,
                    "managersProductsPhotos",
                    glassImage.Product.ManagerId.ToString(),
                    glassImage.ProductId.ToString(), glassImage.Name);

                // Prepare the video content to send to the Flask server
                using (var content = new MultipartFormDataContent())
                {
                    var videoContent = new StreamContent(videoModelDTO.videoFile.OpenReadStream());
                    videoContent.Headers.ContentType = new MediaTypeHeaderValue(videoModelDTO.videoFile.ContentType);
                    content.Add(videoContent, "video", videoModelDTO.videoFile.FileName);

                    // Read glasses image bytes from file
                    byte[] sunglassesImageBytes;
                    using (var fileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                    {
                        sunglassesImageBytes = new byte[fileStream.Length];
                        await fileStream.ReadAsync(sunglassesImageBytes, 0, (int)fileStream.Length);
                    }

                    // Convert image bytes to content and add to request
                    var imageContent = new ByteArrayContent(sunglassesImageBytes);
                    imageContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    content.Add(imageContent, "image", glassImage.Name); // Assuming glassImage.Name is the filename

                    // Make the request to the Flask server
                    var response = await _httpClient.PostAsync("https://7f21-196-157-38-166.ngrok-free.app/process_video", content);

                    if (response.IsSuccessStatusCode)
                    {
                        // Read the processed video bytes from response
                        var videoBytes = await response.Content.ReadAsByteArrayAsync();

                        // Return the video file
                        return File(videoBytes, "video/mp4", "processed_video.mp4");
                    }
                    else
                    {
                        return BadRequest("Failed to process video.");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}
