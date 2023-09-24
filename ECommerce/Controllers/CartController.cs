using ECommerce.Models.DTOs;
using ECommerce.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository cartRepositoryService;

        public CartController(ICartRepository cartRepositoryService)
        {
            this.cartRepositoryService = cartRepositoryService;
        }

        [HttpPost]
        [Authorize(Roles ="Customer")]
        public async Task<IActionResult> AddToCart([FromBody] AddProductItemToCartDto addProductItemToCartDto)
        {
            var result = await cartRepositoryService.AddToCart(addProductItemToCartDto);
            return result ? Ok() : BadRequest();
        }

        [HttpGet("{customerId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetCartProductItems([FromRoute] Guid customerId)
        {
            var cartItemsList = await cartRepositoryService.GetAll(customerId);
            return cartItemsList != null ? Ok(cartItemsList) : NotFound();
        }

        [HttpDelete("{cartProductItemId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> DeleteFromCart([FromRoute] Guid cartProductItemId)
        {
            var result = await cartRepositoryService.DeleteCartItem(cartProductItemId);
            return result ? Ok() : BadRequest();
        }

        [HttpPut]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> UpdateCartProductItem([FromBody] UpdateCartProductItemDto updateCartProductItemDto)
        {
            var result = await cartRepositoryService.UpdateCartItem(updateCartProductItemDto);
            return result ? Ok() : BadRequest();
        }

        [HttpGet("{customerId}/{productItemId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> IsProductItemInCart([FromRoute] Guid customerId, [FromRoute] Guid productItemId)
        {
            var result = await cartRepositoryService.IsProductItemInCart(productItemId, customerId);
            return Ok(new { IsAvailable = result });
        }
    }
}
