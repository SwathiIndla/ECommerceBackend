namespace ECommerce.Models.DTOs
{
    public class PaginatedSearchResultsDto
    {
        public bool MultipleCategories { get; set; }
        public int TotalSearchResults { get; set; }
        public List<ProductItemCardDto> SearchResults { get; set; } = null!;
    }
}
