using ECommerce.DbContext;
using ECommerce.Models.DTOs;
using ECommerce.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilterController : ControllerBase
    {
        private readonly IProductRepository productRepositoryService;

        public FilterController(IProductRepository productRepositoryService)
        {
            this.productRepositoryService = productRepositoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductCards([FromQuery] FilterProductsQueryParametersDto filterConditions, [FromQuery] SortProductsDto sortConditions)
        {
            var productItems = await productRepositoryService.FilterProducts(filterConditions, sortConditions);
            return productItems.TotalFilterResults > 0 ? Ok(productItems) : NotFound();
        }
    }
}
