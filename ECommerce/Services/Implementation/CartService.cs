using AutoMapper;
using ECommerce.DbContext;
using ECommerce.Models.Domain;
using ECommerce.Models.DTOs;
using ECommerce.Repository.Interface;
using ECommerce.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Services.Implementation
{
    public class CartService : ICartService
    {
        private readonly IMapper mapper;
        private readonly ICartRepository cartRepository;
        private readonly IProductRepository productRepository;
        private readonly ISellerRepository sellerRepository;
        private readonly ISaveChangesRepository saveChangesRepository;
        private readonly ICustomerRepository customerRepository;

        public CartService(IMapper mapper, ICartRepository cartRepository, 
            IProductRepository productRepository, ISellerRepository sellerRepository, ISaveChangesRepository saveChangesRepository, 
            ICustomerRepository customerRepository)
        {
            this.mapper = mapper;
            this.cartRepository = cartRepository;
            this.productRepository = productRepository;
            this.sellerRepository = sellerRepository;
            this.saveChangesRepository = saveChangesRepository;
            this.customerRepository = customerRepository;
        }

        public async Task<AddToCartResultDto> AddToCart(AddProductItemToCartDto addProductItemToCartDto)
        {
            var result = new AddToCartResultDto { Result = false, CartProductItem = null };
            var userCart = await cartRepository.GetCartOfCustomer(addProductItemToCartDto.CustomerId);
            var productItem = await productRepository.GetProductItemById(addProductItemToCartDto.ProductItemId);
            var seller = await sellerRepository.GetSellerAsync(addProductItemToCartDto.SellerId);
            if (userCart != null && productItem != null && seller != null)
            {
                var cartProductItemDomain = mapper.Map<CartProductItem>(addProductItemToCartDto);
                cartProductItemDomain.CartId = userCart.CartId;
                cartProductItemDomain.CartProductItemId = Guid.NewGuid();
                await cartRepository.AddProductItemToCart(cartProductItemDomain);
                await saveChangesRepository.AsynchronousSaveChanges();
                result.Result = true;
                result.CartProductItem = mapper.Map<CartProductItemDto>(cartProductItemDomain);
            }
            return result;
        }

        public async Task<bool> DeleteCartItem(Guid cartProductItemId)
        {
            var result = false;
            var cartProductItem = await cartRepository.GetCartProductItem(cartProductItemId);
            if (cartProductItem != null)
            {
                cartRepository.RemoveProductItemFromCart(cartProductItem);
                await saveChangesRepository.AsynchronousSaveChanges();
                result = true;
            }
            return result;
        }

        public async Task<List<CartProductItemDto>?> GetAll(Guid customerId)
        {
            List<CartProductItemDto>? result = null;
            var customer = await customerRepository.GetCustomerById(customerId);
            var customerCart = await cartRepository.GetCartOfCustomer(customerId);
            if (customer != null && customerCart != null)
            {
                var cartProductItems = await cartRepository.GetAllProductsInCart(customerCart.CartId);
                result = mapper.Map<List<CartProductItemDto>>(cartProductItems);
            }
            return result;
        }

        public async Task<bool> UpdateCartItem(UpdateCartProductItemDto updateCartProductItemDto)
        {
            var result = false;
            var cartProductItem = await cartRepository.GetCartProductItem(updateCartProductItemDto.CartProductItemId);
            if (cartProductItem != null)
            {
                cartProductItem.Quantity = updateCartProductItemDto.Quantity;
                await saveChangesRepository.AsynchronousSaveChanges();
                result = true;
            }
            return result;
        }

        public async Task<bool> IsProductItemInCart(Guid productItemId, Guid customerId)
        {
            var result = false;
            var customerCart = await cartRepository.GetCartOfCustomer(customerId);
            if (customerCart != null)
            {
                var cartProductItem = await cartRepository.IsProductItemAvailableInCart(customerCart.CartId, productItemId);
                result = cartProductItem != null;
            }
            return result;
        }
    }
}
