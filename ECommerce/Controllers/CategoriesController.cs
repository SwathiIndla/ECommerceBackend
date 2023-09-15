using AutoMapper;
using ECommerce.Models.DTOs;
using ECommerce.Repository;
using ECommerce.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoriesRepository categoriesRepositoryService;
        private readonly IMapper mapper;

        public CategoriesController(ICategoriesRepository categoriesRepositoryService, IMapper mapper)
        {
            this.categoriesRepositoryService = categoriesRepositoryService;
            this.mapper = mapper;
        }

        [HttpGet("hierarchy")]
        public async Task<IActionResult> GetCategoriesHierarchy()
        {
            var hierarchy = await categoriesRepositoryService.GetCategoryHierarchyAsync();
            var hierarchyDto = mapper.Map<List<CategoryDto>>(hierarchy);
            return Ok(hierarchyDto);
        }

        [HttpGet("properties/{categoryId}")]
        public async Task<IActionResult> GetPropertiesOfCategory([FromRoute] Guid categoryId)
        {
            var propertiesDomain = await categoriesRepositoryService.GetPropertiesOfCategoryAsync(categoryId);
            var propertiesDto = mapper.Map<List<PropertyNameValueDto>>(propertiesDomain);
            return Ok(propertiesDto);
        }

        [HttpGet("brands/{categoryId}")]
        public async Task<IActionResult> GetBrandsOfCategory([FromRoute] Guid categoryId)
        {
            var brands = await categoriesRepositoryService.GetBrandsAsync(categoryId);
            var brandsDto = mapper.Map<List<BrandDto>>(brands);
            return Ok(brandsDto);
        }
    }
}
