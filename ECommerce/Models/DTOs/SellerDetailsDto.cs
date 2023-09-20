namespace ECommerce.Models.DTOs
{
    public class SellerDetailsDto
    {
        public Guid SellerId { get; set; }
        public string SellerName { get; set; } = null!;
    }
}
