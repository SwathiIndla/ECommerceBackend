namespace ECommerce.Models.DTOs
{
    public class OrderedItemDto
    {
        public Guid OrderedItemId { get; set; }
        public Guid ProductItemId { get; set; }
        public Guid OrderId { get; set; }
        public string ProductItemName { get; set; } = null!;
        public long Quantity { get; set; }
        public decimal Price { get; set; }
        public string? ProductItemImage { get; set; }
    }
}
