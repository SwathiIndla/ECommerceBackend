using ECommerce.Models.Domain;

namespace ECommerce.Models.DTOs
{
    public class ProductItemDetailedPageDto
    {
        public ProductItemCardDto? ProductItemDetails { get; set; }
        public Dictionary<string,List<string>> AvailableVariantOptions { get; set; } = null!;
    }
}
