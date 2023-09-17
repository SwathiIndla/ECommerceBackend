using ECommerce.Repository;
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
            return Ok(detailedProductItem);
        }
    }
}
