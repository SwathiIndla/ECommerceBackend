using ECommerce.Models.DTOs;

namespace ECommerce.Repository
{
    public interface IReviewRepository
    {
        Task<ReviewDto> IsReviewPresent(Guid customerId, Guid productId);
        Task<ReviewDto?> AddReview(AddReviewRequestDto addReviewDto);
        Task<ReviewDto?> EditReview(EditReviewRequestDto editReviewRequestDto);
        Task<bool> DeleteReview(Guid productReviewId);
        Task<ReviewSummaryDto?> GetAllReviews(Guid productId, bool sortOnRatingAsc, int page);
        Task<bool> IsProductReviewable(Guid customerId, Guid productId);
    }
}
