using ECommerce.Models.DTOs;

namespace ECommerce.Repository
{
    public interface IReviewRepository
    {
        Task<ReviewDto> IsReviewPresent(Guid customerId, Guid productId);
        Task<bool> AddReview(AddReviewRequestDto reviewDto);
        Task<ReviewDto?> EditReview(EditReviewRequestDto editReviewRequestDto);
        Task<bool> DeleteReview(Guid productReviewId);
        Task<List<ReviewDto>> GetAllReviews(Guid productId, bool sortOnRatingAsc);
    }
}
