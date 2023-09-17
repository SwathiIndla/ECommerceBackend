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

        [HttpGet("mobiles")]
        public async Task<IActionResult> GetMobilesProductCards([FromQuery] FilterMobilesDto filterConditions)
        {
            var productItems = await productRepositoryService.FilterMobiles(filterConditions);
            return productItems.Count > 0 ? Ok(productItems) : NotFound();
        }
    }
}
