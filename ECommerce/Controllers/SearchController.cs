using ECommerce.DbContext;
using ECommerce.Models.Domain;
using ECommerce.Models.DTOs;
using ECommerce.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly IProductRepository productRepositoryService;

        public SearchController(IProductRepository productRepositoryService)
        {
            this.productRepositoryService = productRepositoryService;
        }

        [HttpGet]
        public async Task<IActionResult> SearchProducts([FromQuery] string? search = null)
        {
            var searchResults = !string.IsNullOrEmpty(search) ? await productRepositoryService.SearchProductItem(search) : new Dictionary<Guid, List<ProductItemCardDto>>();
            return  searchResults.Count > 0 ? Ok(searchResults) : NotFound();
        }
    }
}
