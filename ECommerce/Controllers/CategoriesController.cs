using AutoMapper;
using ECommerce.Models.DTOs;
using ECommerce.Services;
using ECommerce.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    /// <summary>
    /// This API Controller handles all the logic related to the Categories
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoriesService categoriesRepositoryService;
        private readonly IMapper mapper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="categoriesRepositoryService"></param>
        /// <param name="mapper"></param>
        public CategoriesController(ICategoriesService categoriesRepositoryService, IMapper mapper)
        {
            this.categoriesRepositoryService = categoriesRepositoryService;
            this.mapper = mapper;
        }

        /// <summary>
        /// Gets the complete categories Hierarchy
        /// </summary>
        /// <returns>Returns 200Ok response with List(CategoryDto)</returns>
        [HttpGet("hierarchy")]
        public async Task<IActionResult> GetCategoriesHierarchy()
        {
            try
            {
                var hierarchy = await categoriesRepositoryService.GetCategoryHierarchyAsync();
                var hierarchyDto = mapper.Map<List<CategoryDto>>(hierarchy);
                return Ok(hierarchyDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        /// <summary>
        /// Gets all the properties specific to the category
        /// </summary>
        /// <param name="categoryId">Guid</param>
        /// <returns>Returns 200Ok response with List(PropertyNameValueDto)</returns>
        [HttpGet("properties/{categoryId}")]
        public async Task<IActionResult> GetPropertiesOfCategory([FromRoute] Guid categoryId)
        {
            try
            {
                var propertiesDomain = await categoriesRepositoryService.GetPropertiesOfCategoryAsync(categoryId);
                var propertiesDto = mapper.Map<List<PropertyNameValueDto>>(propertiesDomain);
                return Ok(propertiesDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        /// <summary>
        /// Gets all the brands present in the particular category
        /// </summary>
        /// <param name="categoryId">Guid</param>
        /// <returns>Returns 200Ok response with List(BrandDto)</returns>
        [HttpGet("brands/{categoryId}")]
        public async Task<IActionResult> GetBrandsOfCategory([FromRoute] Guid categoryId)
        {
            try
            {
                var brands = await categoriesRepositoryService.GetBrandsAsync(categoryId);
                var brandsDto = mapper.Map<List<BrandDto>>(brands);
                return Ok(brandsDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }
    }
}
