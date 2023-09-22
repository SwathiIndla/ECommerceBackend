using ECommerce.Models.DTOs;

namespace ECommerce.Repository
{
    public interface ICartRepository
    {
        Task<bool> AddToCart(AddProductItemToCartDto addProductItemToCartDto);
        Task<List<CartProductItemDto>?> GetAll(Guid customerId);
        Task<bool> DeleteCartItem(Guid cartProductItemId);
        Task<bool> UpdateCartItem(UpdateCartProductItemDto updateCartProductItemDto);
    }
}
