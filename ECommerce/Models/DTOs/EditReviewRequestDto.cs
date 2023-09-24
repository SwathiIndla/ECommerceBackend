namespace ECommerce.Models.DTOs
{
    public class EditReviewRequestDto
    {
        public Guid ProductReviewId { get; set; }
        public string? Review { get; set; }
        public decimal Rating { get; set; }
    }
}
