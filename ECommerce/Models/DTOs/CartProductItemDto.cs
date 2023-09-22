namespace ECommerce.Models.DTOs
{
    public class CartProductItemDto
    {
        public Guid CartProductItemId { get; set; }
        public Guid ProductItemId { get; set; }
        public long Quantity { get; set; }
        public string? ProductItemImage { get; set; }
        public string ProductItemName { get; set; } = null!;
        public decimal Price { get; set; }
        public string SellerName { get; set;} = null!;
    }
}
