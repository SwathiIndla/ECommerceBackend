using ECommerce.Models.DTOs;
using ECommerce.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    /// <summary>
    /// This API Controller handles all the logic related to Product page
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService productRepositoryService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productRepositoryService"></param>
        public ProductController(IProductService productRepositoryService)
        {
            this.productRepositoryService = productRepositoryService;
        }

        /// <summary>
        /// Gets the Detailed productItem information
        /// </summary>
        /// <param name="productItemId">Guid</param>
        /// <returns>Returns 200Ok response with ProductItemDetailedPageDto if the productItem exists otherwise returns 404NotFound</returns>
        /// <response code="200">Returns ProductItemDetailedPageDto when successfully found</response>
        /// <response code="404">Returns Not found when the order with given id is not found</response>
        /// <response code="500">Returns Internal Server Error with Message when an exception occurs</response>
        [HttpGet("{productItemId}")]
        [ProducesResponseType(typeof(ProductItemDetailedPageDto),200)]
        public async Task<IActionResult> GetDetailedProductItem([FromRoute] Guid productItemId)
        {
            try
            {
                var detailedProductItem = await productRepositoryService.GetDetailedProductItem(productItemId);
                return detailedProductItem != null ? Ok(detailedProductItem) : NotFound();
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
        /// Gets the Available variants (i.e, different productItems) of a particular product
        /// </summary>
        /// <param name="productId">Guid</param>
        /// <param name="filterConditions">FilterVariantParametersDto object</param>
        /// <param name="featuresDataNeeded">a list of strings</param>
        /// <returns>Returns 200Ok response with ProductVariantDetailedPageDto if the productItems exist otherwise 404NotFound</returns>
        /// <response code="200">Returns ProductVariantDetailedPageDto when successfully found</response>
        /// <response code="404">Returns Not found when the order with given id is not found</response>
        /// <response code="500">Returns Internal Server Error with Message when an exception occurs</response>
        [HttpGet("variant/{productId}")]
        [ProducesResponseType(typeof(ProductVariantDetailedPageDto),200)]
        public async Task<IActionResult> GetProductVariantDetails([FromRoute] Guid productId, [FromQuery] FilterVariantParametersDto filterConditions, [FromQuery] List<string> featuresDataNeeded)
        {
            try
            {
                var finalVariantAndFeatures = await productRepositoryService.FilterProductVariant(productId, filterConditions, featuresDataNeeded);
                return finalVariantAndFeatures.Variants.Count > 0 ? Ok(finalVariantAndFeatures) : NotFound();
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
