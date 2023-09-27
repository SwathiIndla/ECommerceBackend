namespace ECommerce.Models.DTOs
{
    public class CreateOrderRequestDto
    {
        public Guid CustomerId { get; set; }
        public string ShippingAddress { get; set; } = null!;
        public List<Guid> CartProductItemIds { get; set; } = null!;
    }
}
