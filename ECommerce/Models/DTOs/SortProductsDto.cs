namespace ECommerce.Models.DTOs
{
    public class SortProductsDto
    {
        public bool SortOnPrice { get; set; } = false;
        public bool SortByPriceAsc { get; set; } = true;
        public bool SortOnRating { get; set; } = true;
        public bool SortByRatingAsc { get;set; } = false;
    }
}
