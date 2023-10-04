namespace ECommerce.Models.DTOs
{
    public class ReviewSummaryDto
    {
        public decimal AverageRating { get; set; }
        public int TotalRatings { get; set; }
        public int TotalReviews { get; set; }
        public int FiveStarRatings { get; set; }
        public int FourStarRatings { get; set; }
        public int ThreeStarRatings { get; set; }
        public int TwoStarRatings { get; set; }
        public int OneStarRatings { get; set; }
        public List<ReviewDto> Reviews { get; set; } = new List<ReviewDto>();
    }
}
