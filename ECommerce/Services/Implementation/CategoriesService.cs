using AutoMapper;
using ECommerce.DbContext;
using ECommerce.Models.Domain;
using ECommerce.Repository.Interface;
using ECommerce.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Services.Implementation
{
    public class CategoriesService : ICategoriesService
    {
        private readonly ICategoriesRepository categoriesRepository;

        public CategoriesService(ICategoriesRepository categoriesRepository)
        {
            this.categoriesRepository = categoriesRepository;
        }

        public async Task<List<Brand>> GetBrandsAsync(Guid categoryId)
        {
            return await categoriesRepository.GetBrandsOfCategory(categoryId);
        }

        public async Task<List<Category>> GetCategoryHierarchyAsync()
        {
            return await categoriesRepository.GetCategoryHierarchy();
        }

        public async Task<List<PropertyName>> GetPropertiesOfCategoryAsync(Guid categoryId)
        {
            return await categoriesRepository.GetPropertiesOfCategory(categoryId);
        }


    }
}
