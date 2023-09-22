namespace ECommerce.Models.DTOs
{
    public class UpdateCartProductItemDto
    {
        public Guid CartProductItemId { get; set; }
        public long Quantity { get; set; }
    }
}
