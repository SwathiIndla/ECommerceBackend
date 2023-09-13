using AutoMapper;
using ECommerce.Models.Domain;
using ECommerce.Models.DTOs;
using ECommerce.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataSeedingController : ControllerBase
    {
        private readonly IAdminRepository adminRepositoryService;
        private readonly IMapper mapper;

        public DataSeedingController(IAdminRepository adminRepositoryService, IMapper mapper)
        {
            this.adminRepositoryService = adminRepositoryService;
            this.mapper = mapper;
        }

        [HttpPost("AddCategories")]
        public async Task<IActionResult> AddCategories([FromBody] List<AddCategoryRequestDto> addCategoriesRequestDto)
        {
            try
            {
                var newCategoriesList = new List<Category>();
                foreach (var category in addCategoriesRequestDto)
                {
                    var categoryDomain = mapper.Map<Category>(category);
                    categoryDomain.CategoryId = Guid.NewGuid();
                    var newCategory = await adminRepositoryService.AddCategory(categoryDomain);
                    newCategoriesList.Add(newCategory);
                }
                return Ok(newCategoriesList);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("AddBrands/{categoryName}")]
        public async Task<IActionResult> AddBrands([FromRoute] string categoryName,[FromBody] List<AddBrandRequestDto> addBrandsRequestDto)
        {
            try
            {
                var brandCategoryAssociation = new List<BrandCategoryDto>();
                var category = await adminRepositoryService.GetCategoryByName(categoryName);
                if (category != null)
                {
                    var errors = new List<String>();
                    foreach (var brand in addBrandsRequestDto)
                    {
                        var existingBrand = await adminRepositoryService.GetBrandByName(brand.BrandName);
                        if (existingBrand == null)
                        {
                            var brandDomain = mapper.Map<Brand>(brand);
                            brandDomain.BrandId = Guid.NewGuid();
                            var newBrandCategory = await adminRepositoryService.AddBrand(brandDomain, category);
                            var newBrandCategoryDto = mapper.Map<BrandCategoryDto>(newBrandCategory);
                            brandCategoryAssociation.Add(newBrandCategoryDto);
                        }
                        else
                        {
                            errors.Add($"{brand.BrandName} already exists");
                        }
                    }
                    return Ok(new { brandCategoryAssociation, errors });
                }
                return NotFound(new { Message = "Category Does not exist" });
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("AddProperties/{categoryName}")]
        public async Task<IActionResult> AddProperties([FromRoute] string categoryName, [FromBody] List<string> properties)
        {
            var category = await adminRepositoryService.GetCategoryByName(categoryName);
            if(category != null)
            {
                if(properties.Count > 0)
                {
                    var newProperties = new List<PropertyNameDto>();
                    var errors = new List<string>();
                    foreach(var property in properties)
                    {
                        if(property != "" && property != null)
                        {
                            var existingProperty = await adminRepositoryService.GetPropertyNameByName(property, category.CategoryId);
                            if(existingProperty == null )
                            {
                                var newProperty = await adminRepositoryService.AddPropertyName(property, category);
                                var newPropertyDto = mapper.Map<PropertyNameDto>(newProperty);
                                newProperties.Add(newPropertyDto);
                            }
                            else
                            {
                                errors.Add($"{property} already exists for the given category");
                            }
                        }
                        else if(errors.Count == 0)
                        {
                            errors.Add("Property name cannot be empty");
                        }
                    }
                    return Ok(new { newProperties, errors });
                }
                return BadRequest(new {Message = "List of properties cannot be empty. Provide the property names of the category to add to the database"});
            }
            return NotFound(new { Message = "Category Does not exist" });
        }
    }
}
