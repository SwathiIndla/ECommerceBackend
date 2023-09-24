namespace ECommerce.Models.DTOs
{
    public class AddReviewRequestDto
    {
        public Guid CustomerId { get; set; }
        public Guid ProductId { get; set; }
        public string? Review { get; set; }
        public decimal Rating { get; set; }
    }
}
