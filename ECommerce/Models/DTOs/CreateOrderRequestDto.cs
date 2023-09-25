namespace ECommerce.Models.DTOs
{
    public class CreateOrderRequestDto
    {
        public Guid CustomerId { get; set; }
        public string ShippingAddress { get; set; }
        public List<Guid> CartProductItemIds { get; set; } = new List<Guid>();
    }
}
