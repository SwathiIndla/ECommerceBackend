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
        //This API will return the complete details of the productItem
        public async Task<IActionResult> GetDetailedProductItem([FromRoute] Guid productItemId)
        {
            var detailedProductItem = await productRepositoryService.GetDetailedProductItem(productItemId);
            return  detailedProductItem != null ? Ok(detailedProductItem) : NotFound();
        }

        [HttpGet("variant/{productId}")]
        //This API will return the available variants of the product
        public async Task<IActionResult> GetProductVariantDetails([FromRoute] Guid productId, [FromQuery] FilterVariantParametersDto filterConditions, [FromQuery] List<string> featuresDataNeeded)
        {
            var finalVariantAndFeatures = await productRepositoryService.FilterProductVariant(productId, filterConditions, featuresDataNeeded);
            return finalVariantAndFeatures.Variants.Count > 0 ? Ok(finalVariantAndFeatures) : NotFound();
        }
    }
}
