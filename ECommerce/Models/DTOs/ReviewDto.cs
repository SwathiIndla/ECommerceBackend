namespace ECommerce.Models.DTOs
{
    public class ReviewDto
    {
        public Guid ProductReviewId { get; set; }
        public string CustomerName { get; set; } = null!;
        public string? Review { get; set; }
        public decimal Rating { get; set; }
    }
}
