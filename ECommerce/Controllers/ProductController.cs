using ECommerce.Models.DTOs;
using ECommerce.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository productRepositoryService;

        public ProductController(IProductRepository productRepositoryService)
        {
            this.productRepositoryService = productRepositoryService;
        }

        [HttpGet("{productItemId}")]
        public async Task<IActionResult> GetDetailedProductItem([FromRoute] Guid productItemId)
        {
            var detailedProductItem = await productRepositoryService.GetDetailedProductItem(productItemId);
            return  detailedProductItem != null ? Ok(detailedProductItem) : NotFound();
        }

        [HttpGet("variant/{productId}")]
        public async Task<IActionResult> GetProductVariantDetails([FromRoute] Guid productId, [FromQuery] FilterVariantParametersDto filterConditions, [FromQuery] List<string> featuresDataNeeded)
        {
            var finalVariantAndFeatures = await productRepositoryService.FilterProductVariant(productId, filterConditions, featuresDataNeeded);
            return finalVariantAndFeatures.Variants.Count > 0 ? Ok(finalVariantAndFeatures) : NotFound();
        }

        [HttpGet("{customerId}/{productItemId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> IsProductItemInCart([FromRoute] Guid customerId, [FromRoute] Guid productItemId)
        {
            var result = await productRepositoryService.IsProductItemInCart(productItemId, customerId);
            return result ? Ok(new { IsAvailable = result }) : NotFound(new {IsAvailable = false});
        }
    }
}
