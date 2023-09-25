using AutoMapper;
using ECommerce.DbContext;
using ECommerce.Models.Domain;
using ECommerce.Models.DTOs;
using ECommerce.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Services
{
    public class CartRepositoryService : ICartRepository
    {
        private readonly EcommerceContext dbContext;
        private readonly IMapper mapper;

        public CartRepositoryService(EcommerceContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public async Task<AddToCartResultDto> AddToCart(AddProductItemToCartDto addProductItemToCartDto)
        {
            AddToCartResultDto result = new AddToCartResultDto { Result = false, CartProductItem = null};
            var userCart = await dbContext.Carts.FirstOrDefaultAsync(cart => cart.CustomerId == addProductItemToCartDto.CustomerId);
            var productItem = await dbContext.ProductItemDetails.FirstOrDefaultAsync(item => item.ProductItemId == addProductItemToCartDto.ProductItemId);
            var seller = await dbContext.Sellers.FirstOrDefaultAsync(seller => seller.SellerId == addProductItemToCartDto.SellerId);
            if(userCart != null && productItem != null && seller != null)
            {
                var cartProductItemDomain = mapper.Map<CartProductItem>(addProductItemToCartDto);
                cartProductItemDomain.CartId = userCart.CartId;
                cartProductItemDomain.CartProductItemId = Guid.NewGuid();
                await dbContext.CartProductItems.AddAsync(cartProductItemDomain);
                await dbContext.SaveChangesAsync();
                result.Result = true;
                result.CartProductItem = mapper.Map<CartProductItemDto>(cartProductItemDomain);
            }
            return result;
        }

        public async Task<bool> DeleteCartItem(Guid cartProductItemId)
        {
            var result = false;
            var cartProductItem = await dbContext.CartProductItems.FirstOrDefaultAsync(item => item.CartProductItemId == cartProductItemId);
            if(cartProductItem != null)
            {
                dbContext.CartProductItems.Remove(cartProductItem);
                await dbContext.SaveChangesAsync();
                result = true;
            }
            return result;
        }

        public async Task<List<CartProductItemDto>?> GetAll(Guid customerId)
        {
            List<CartProductItemDto>? result = null;
            var customer = await dbContext.CustomerCredentials.FirstOrDefaultAsync(user => user.CustomerId == customerId);
            var customerCart = await dbContext.Carts.FirstOrDefaultAsync(cart => cart.CustomerId == customerId);
            if (customer != null && customerCart != null)
            {
                var cartProductItems = await dbContext.CartProductItems.Include(item => item.Seller).Include(item => item.ProductItem).Where(item => item.CartId == customerCart.CartId).ToListAsync();
                result = mapper.Map<List<CartProductItemDto>>(cartProductItems);
            }
            return result;
        }

        public async Task<bool> UpdateCartItem(UpdateCartProductItemDto updateCartProductItemDto)
        {
            var result = false;
            var cartProductItem = await dbContext.CartProductItems.FirstOrDefaultAsync(item => item.CartProductItemId == updateCartProductItemDto.CartProductItemId);
            if(cartProductItem != null)
            {
                cartProductItem.Quantity = updateCartProductItemDto.Quantity;
                await dbContext.SaveChangesAsync();
                result = true;
            }
            return result;
        }

        public async Task<bool> IsProductItemInCart(Guid productItemId, Guid customerId)
        {
            var result = false;
            var customerCart = await dbContext.Carts.Include(cart => cart.CartProductItems).FirstOrDefaultAsync(cart => cart.CustomerId == customerId);
            if (customerCart != null)
            {
                var cartProductItem = await dbContext.CartProductItems.Include(item => item.Cart)
                    .FirstOrDefaultAsync(item => item.Cart.CartId == customerCart.CartId && item.ProductItemId == productItemId);
                result = cartProductItem != null;
            }
            return result;
        }
    }
}
