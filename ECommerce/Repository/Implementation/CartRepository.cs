using ECommerce.DbContext;
using ECommerce.Models.Domain;
using ECommerce.Models.DTOs;
using ECommerce.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Repository.Implementation
{
    public class CartRepository : ICartRepository
    {
        private readonly EcommerceContext dbContext;

        public CartRepository(EcommerceContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<Cart?> GetCartOfCustomer(Guid customerId)
        {
            return await dbContext.Carts.FirstOrDefaultAsync(cart => cart.CustomerId == customerId);
        }
        public async Task AddProductItemToCart(CartProductItem cartProductItem)
        {
            await dbContext.CartProductItems.AddAsync(cartProductItem);
        }

        public async Task<List<CartProductItem>> GetAllProductsInCart(Guid cartId)
        {
            return await dbContext.CartProductItems.Include(item => item.Seller).Include(item => item.ProductItem).Where(item => item.CartId == cartId).ToListAsync();
        }

        public async Task<CartProductItem?> GetCartProductItem(Guid cartProductItemId)
        {
            return await dbContext.CartProductItems.FirstOrDefaultAsync(item => item.CartProductItemId == cartProductItemId);
        }

        public void RemoveProductItemFromCart(CartProductItem cartProductItem)
        {
            dbContext.CartProductItems.Remove(cartProductItem);
        }

        public async Task<CartProductItem?> IsProductItemAvailableInCart(Guid cartId, Guid productItemId)
        {
            return await dbContext.CartProductItems.Include(item => item.Cart).FirstOrDefaultAsync(item => item.Cart.CartId == cartId && item.ProductItemId == productItemId);
        }

        public async Task<List<CartProductItem>> GetCartProductItemsList(CreateOrderRequestDto createOrderRequestDto)
        {
            return await dbContext.CartProductItems.Include(item => item.ProductItem).Where(item => createOrderRequestDto.CartProductItemIds.Contains(item.CartProductItemId)).ToListAsync();
        }
    }
}
