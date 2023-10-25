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
        /// <response code="200">Returns AddToCartResultDto if the productItem is successfully added to cart</response>
        /// <response code="400">Returns Bad request with AddToCartResultDto if the productItem is not added to cart</response>
        /// <response code="401">Returns Unauthorized Status code when the token sent is invalid or when token is missing</response>
        /// <response code="403">Returns Forbidden Status code when the logged in user does not have Customer role</response>
        /// <response code="500">Returns Internal Server Error with Message when an exception occurs</response>
        [HttpPost]
        [ProducesResponseType(typeof(AddToCartResultDto), 200)]
        [ProducesResponseType(typeof(AddToCartResultDto), 400)]
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
                return StatusCode(StatusCodes.Status500InternalServerError, (new
                {
                    ex.Message
                }));
            }
        }

        /// <summary>
        /// Retrieve all the productItems present in the customer's cart
        /// </summary>
        /// <param name="customerId">Guid</param>
        /// <returns>Returns 200Ok response with List(CartProductItemDto) if present otherwise 404NotFound</returns>
        /// <response code="200">Returns List of cartProductItemDto</response>
        /// <response code="404">Returns Not found when there are no items in cart of customer</response>
        /// <response code="401">Returns Unauthorized Status code when the token sent is invalid or when token is missing</response>
        /// <response code="403">Returns Forbidden Status code when the logged in user does not have Customer role</response>
        /// <response code="500">Returns Internal Server Error with Message when an exception occurs</response>
        [HttpGet("{customerId}")]
        [ProducesResponseType(typeof(List<CartProductItemDto>), 200)]
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
                return StatusCode(StatusCodes.Status500InternalServerError, (new
                {
                    ex.Message
                }));
            }
        }

        /// <summary>
        /// Delete the respective productItem from the customer's cart
        /// </summary>
        /// <param name="cartProductItemId">Guid</param>
        /// <returns>Returns 200Ok response if deletion is successful otherwise 400BadRequest</returns>
        /// <response code="200">When the product is deleted from the cart successfully</response>
        /// <response code="400">Returns Bad request when the item could not be deleted</response>
        /// <response code="401">Returns Unauthorized Status code when the token sent is invalid or when token is missing</response>
        /// <response code="403">Returns Forbidden Status code when the logged in user does not have Customer role</response>
        /// <response code="500">Returns Internal Server Error with Message when an exception occurs</response>
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
                return StatusCode(StatusCodes.Status500InternalServerError, (new
                {
                    ex.Message
                }));
            }
        }

        /// <summary>
        /// Updates the quantity of the productItem in customer's cart
        /// </summary>
        /// <param name="updateCartProductItemDto">UpdateCartProductItemDto object</param>
        /// <returns>Returns 200Ok response if updation is successful otherwise 400BadRequest</returns>
        /// <response code="200">When the cart is updated successfully</response>
        /// <response code="400">Returns Bad request when updation fails</response>
        /// <response code="401">Returns Unauthorized Status code when the token sent is invalid or when token is missing</response>
        /// <response code="403">Returns Forbidden Status code when the logged in user does not have Customer role</response>
        /// <response code="500">Returns Internal Server Error with Message when an exception occurs</response>
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
                return StatusCode(StatusCodes.Status500InternalServerError, (new
                {
                    ex.Message
                }));
            }
        }

        /// <summary>
        /// Checks if the productItem is already present in cart
        /// </summary>
        /// <param name="customerId">Guid</param>
        /// <param name="productItemId">Guid</param>
        /// <returns>Returns 200Ok response with IsAvailable flag in a object</returns>
        /// <response code="200">Returns an IsAvailable flag</response>
        /// <response code="401">Returns Unauthorized Status code when the token sent is invalid or when token is missing</response>
        /// <response code="403">Returns Forbidden Status code when the logged in user does not have Customer role</response>
        /// <response code="500">Returns Internal Server Error with Message when an exception occurs</response>
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
                return StatusCode(StatusCodes.Status500InternalServerError, (new
                {
                    ex.Message
                }));
            }
        }
    }
}
