using ECommerce.Models.DTOs;
using ECommerce.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    /// <summary>
    /// This API Controller handles all the logic related to the Cart
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService cartRepositoryService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cartRepositoryService"></param>
        public CartController(ICartService cartRepositoryService)
        {
            this.cartRepositoryService = cartRepositoryService;
        }

        /// <summary>
        /// Adds productItem to the customer's cart
        /// </summary>
        /// <param name="addProductItemToCartDto">AddProductItemToCartDto Object</param>
        /// <returns>Returns 200Ok response with AddToCartResultDto object if product item is successfully added to cart otherwise 400BadRequest</returns>
        [HttpPost]
        [Authorize(Roles ="Customer")]
        public async Task<IActionResult> AddToCart([FromBody] AddProductItemToCartDto addProductItemToCartDto)
        {
            try
            {
                var result = await cartRepositoryService.AddToCart(addProductItemToCartDto);
                return result.Result ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        /// <summary>
        /// Retrieve all the productItems present in the customer's cart
        /// </summary>
        /// <param name="customerId">Guid</param>
        /// <returns>Returns 200Ok response with List(CartProductItemDto) if present otherwise 404NotFound</returns>
        [HttpGet("{customerId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetCartProductItems([FromRoute] Guid customerId)
        {
            try
            {
                var cartItemsList = await cartRepositoryService.GetAll(customerId);
                return cartItemsList != null ? Ok(cartItemsList) : NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        /// <summary>
        /// Delete the respective productItem from the customer's cart
        /// </summary>
        /// <param name="cartProductItemId">Guid</param>
        /// <returns>Returns 200Ok response if deletion is successful otherwise 400BadRequest</returns>
        [HttpDelete("{cartProductItemId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> DeleteFromCart([FromRoute] Guid cartProductItemId)
        {
            try
            {
                var result = await cartRepositoryService.DeleteCartItem(cartProductItemId);
                return result ? Ok() : BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        /// <summary>
        /// Updates the quantity of the productItem in customer's cart
        /// </summary>
        /// <param name="updateCartProductItemDto">UpdateCartProductItemDto object</param>
        /// <returns>Returns 200Ok response if updation is successful otherwise 400BadRequest</returns>
        [HttpPut]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> UpdateCartProductItem([FromBody] UpdateCartProductItemDto updateCartProductItemDto)
        {
            try
            {
                var result = await cartRepositoryService.UpdateCartItem(updateCartProductItemDto);
                return result ? Ok() : BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        /// <summary>
        /// Checks if the productItem is already present in cart
        /// </summary>
        /// <param name="customerId">Guid</param>
        /// <param name="productItemId">Guid</param>
        /// <returns>Returns 200Ok response with IsAvailable flag in a object</returns>
        [HttpGet("IsProductItemInCart/{customerId}/{productItemId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> IsProductItemInCart([FromRoute] Guid customerId, [FromRoute] Guid productItemId)
        {
            try
            {
                var result = await cartRepositoryService.IsProductItemInCart(productItemId, customerId);
                return Ok(new { IsAvailable = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }
    }
}
