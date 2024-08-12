using Application.pagination;
using Core.DTOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IReviewService
    {
        Task<ReviewDtoResponse> AddReview(ReviewDTORequest reviewDTORequest);

        Task<ReviewDtoResponse> UpdateReview(ReviewDtoUpdate reviewDtoUpdate);

        Task<PaginatedList<ReviewDtoResponse>> GetReviewsForProduct(Guid id,
            int pageIndex = 1, int pageSize = 10);

        Task<bool> DeleteReview(Guid id);

    }
}
