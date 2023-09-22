namespace ECommerce.Models.DTOs
{
    public class AddProductItemToCartDto
    {
        public Guid CustomerId { get; set; }
        public Guid ProductItemId { get; set; }
        public long Quantity { get; set; }
        public Guid SellerId { get; set; }
    }
}
