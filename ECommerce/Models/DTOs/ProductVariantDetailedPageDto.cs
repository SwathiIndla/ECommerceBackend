namespace ECommerce.Models.DTOs
{
    public class ProductVariantDetailedPageDto
    {
        public List<ProductItemCardDto> Variants { get; set; } = null!;
        public Dictionary<string,List<string>> AvailableOptions { get; set; } = null!;
    }
}