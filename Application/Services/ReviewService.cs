using Application.Interfaces;
using Application.pagination;
using Core.DTOS;
using Core.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly ApplicationDbContext _db;

        public ReviewService(ApplicationDbContext db)
        {
            this._db = db;
        }
        public async Task<ReviewDtoResponse> AddReview(
            ReviewDTORequest reviewDTORequest)
        {
            Review review = new Review { 
                Comment = reviewDTORequest.Comment,
                ProductId = reviewDTORequest.ProductId,
                Rating = reviewDTORequest.Rating
            
            };

            await _db.Reviews.AddAsync(review);

            await _db.SaveChangesAsync();

            return new ReviewDtoResponse() {
                Id = review.Id, 
                Comment = review.Comment, 
                Rating = review.Rating 
            };
        }


        public async Task<PaginatedList<ReviewDtoResponse>> GetReviewsForProduct(Guid id , 
            int pageIndex = 1 , int pageSize = 10)
        {
            var reviews = await 
                _db
                .Reviews
                .Where(r => r.ProductId == id)
                .Select(r => new ReviewDtoResponse
                {
                    Comment = r.Comment,
                    Id = id,
                    Rating = r.Rating
                } )
                .PaginatedListAsync(pageIndex, pageSize);
            

            if (reviews is null)
                return null;

           

            return reviews;
        }

        public async Task<ReviewDtoResponse> UpdateReview(ReviewDtoUpdate reviewDtoUpdate)
        {
            var productReview = await _db.Reviews.FindAsync(reviewDtoUpdate.Id);

            if (productReview is null)
                return null;

            productReview.Rating = reviewDtoUpdate.Rating;
            productReview.Comment = reviewDtoUpdate.Comment;

            await _db.SaveChangesAsync();

            return new ReviewDtoResponse
            {
                Comment = productReview.Comment,
                Rating = productReview.Rating
            };
        }
        
        public async Task<bool> DeleteReview(Guid id)
        {
           Review ? review = await _db.Reviews.FirstOrDefaultAsync(x => x.Id == id);

            if (review == null)
                return false;

            _db.Reviews.Remove(review);

            await _db.SaveChangesAsync();

            return true;
        }
    }
}
