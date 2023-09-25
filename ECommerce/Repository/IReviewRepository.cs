using ECommerce.Models.DTOs;

namespace ECommerce.Repository
{
    public interface IReviewRepository
    {
        Task<ReviewDto> IsReviewPresent(Guid customerId, Guid productId);
        Task<ReviewDto?> AddReview(AddReviewRequestDto addReviewDto);
        Task<ReviewDto?> EditReview(EditReviewRequestDto editReviewRequestDto);
        Task<bool> DeleteReview(Guid productReviewId);
        Task<List<ReviewDto>> GetAllReviews(Guid productId, bool sortOnRatingAsc);
        Task<bool> IsProductReviewable(Guid customerId, Guid productId);
    }
}
