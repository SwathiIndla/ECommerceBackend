namespace ECommerce.Models.DTOs
{
    public class OrderDto
    {
        public Guid OrderId { get; set; }
        public string ShippingAddress { get; set; } = null!;
        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; } = null!;
        public List<OrderedItemDto> OrderedItems { get; set; } = new List<OrderedItemDto>(); 
    }
}
