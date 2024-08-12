using Application.Interfaces;
using Application.pagination;
using Core.DTOS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlassesApp.Controllers
{
    public class ReviewsController : BaseApiController
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            this._reviewService = reviewService;
        }

        [HttpGet("{ProductId}")]
        public async Task<ActionResult<PaginatedList<ReviewDtoResponse>>>
            ProductReviews(Guid ProductId)
        {
            var productReviews = await _reviewService.GetReviewsForProduct(ProductId);

            if (productReviews == null)
            {
                return NotFound("Product Not Has Reviews");
            }

            return Ok(productReviews);
        }

        [HttpPost("add-review")]
        [Authorize(Roles = "AppUser")]
        public async Task<ActionResult<ReviewDtoResponse>> AddReviewForProduct(
            ReviewDTORequest review)
        {
            var reviewResponse = await _reviewService.AddReview(review);

            return Ok(reviewResponse);
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "AppUser")]

        public async Task<ActionResult<ReviewDtoResponse>>UpdateReview(ReviewDtoUpdate ReviewDtoUpdate){

             var productReviewUpdated =  await _reviewService.UpdateReview(ReviewDtoUpdate);

            if (productReviewUpdated == null)
                return BadRequest("the review not correct");

             return Ok(productReviewUpdated);
            }

        [HttpDelete("{id}")]
        [Authorize(Roles = "AppUser")]

        public async Task<ActionResult<bool>> 
            DeleteProductReview(Guid reviewId)
        {
           bool isDeleted = await _reviewService.DeleteReview(reviewId);

            if (!isDeleted)
            {
                return BadRequest("The review Id Is not correct");
            }

            return isDeleted;
        }
    }
}

