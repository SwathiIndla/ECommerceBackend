using ECommerce.DbContext;
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
        public IActionResult MobilesProductCard(
            [FromQuery] string? search = null,
            [FromQuery] List<Guid>? brands = null,
            [FromQuery] List<string>? colour = null,
            [FromQuery] List<string>? ram = null,
            [FromQuery] List<string>? storage = null,
            [FromQuery] List<string>? battery = null,
            [FromQuery] List<string>? screenSize = null,
            [FromQuery] List<string>? resolution = null,
            [FromQuery] List<string>? primaryCamera = null,
            [FromQuery] List<string>? secondaryCamera = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] int page = 1)
        {
            var productItems = productRepositoryService.FetchMobileProductItems(search, brands, colour, ram, storage, battery, screenSize, resolution, primaryCamera, secondaryCamera, minPrice, maxPrice, page);
            return Ok(productItems);
        }
    }
}
