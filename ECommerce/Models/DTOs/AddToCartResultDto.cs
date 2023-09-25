namespace ECommerce.Models.DTOs
{
    public class AddToCartResultDto
    {
        public bool Result { get; set; }
        public CartProductItemDto? CartProductItem { get; set; }
    }
}
