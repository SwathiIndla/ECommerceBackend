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
        //This API will search the productItems database for the search string provided
        public async Task<IActionResult> SearchProducts([FromQuery] SortProductsDto sortConditions, [FromQuery] string? search = null, [FromQuery] int page = 1)
        {
            var searchResults = !string.IsNullOrEmpty(search) ? await productRepositoryService.SearchProductItem(search, page, sortConditions) : new PaginatedSearchResultsDto();
            return  searchResults.TotalSearchResults > 0 ? Ok(searchResults) : NotFound();
        }
    }
}
