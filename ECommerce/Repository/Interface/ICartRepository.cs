using ECommerce.Models.Domain;
using ECommerce.Models.DTOs;

namespace ECommerce.Repository.Interface
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartOfCustomer(Guid customerId);
        Task AddProductItemToCart(CartProductItem cartProductItem);
        void RemoveProductItemFromCart(CartProductItem cartProductItem);
        Task<CartProductItem?> GetCartProductItem(Guid cartProductItemId);
        Task<List<CartProductItem>> GetAllProductsInCart(Guid cartId);
        Task<CartProductItem?> IsProductItemAvailableInCart(Guid cartId, Guid productItemId);
        Task<List<CartProductItem>> GetCartProductItemsList(CreateOrderRequestDto createOrderRequestDto);
        
    }
}
