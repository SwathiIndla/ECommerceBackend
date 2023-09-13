using ECommerce.DbContext;
using ECommerce.Models.Domain;
using ECommerce.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Reflection.Metadata.Ecma335;

namespace ECommerce.Services
{
    public class AdminRepositoryService : IAdminRepository
    {
        private readonly EcommerceContext dbContext;

        public AdminRepositoryService(EcommerceContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<BrandCategory> AddBrand(Brand brand, Category category)
        {
            await dbContext.Brands.AddAsync(brand);
            await dbContext.SaveChangesAsync();
            var brandCategory = new BrandCategory
            {
                Id = Guid.NewGuid(),
                BrandId = brand.BrandId,
                CategoryId = category.CategoryId
            };
            await dbContext.BrandCategories.AddAsync(brandCategory);
            await dbContext.SaveChangesAsync();
            return brandCategory;
        }

        public async Task<Category> AddCategory(Category category)
        {
            await dbContext.Categories.AddAsync(category);
            await dbContext.SaveChangesAsync();
            return category;
        }

        public async Task<PropertyName> AddPropertyName(string name, Category category)
        {
            var propertyName = new PropertyName
            {
                PropertyId = Guid.NewGuid(),
                PropertyName1 = name,
                CategoryId = category.CategoryId
            };
            await dbContext.PropertyNames.AddAsync(propertyName);
            await dbContext.SaveChangesAsync();
            return propertyName;
        }

        public async Task<Brand?> GetBrandByName(string name)
        {
            return await dbContext.Brands.FirstOrDefaultAsync(x => x.BrandName == name);
        }

        public async Task<Category?> GetCategoryByName(string name)
        {
            return await dbContext.Categories.FirstOrDefaultAsync(x => x.CategoryName == name);
        }

        public async Task<PropertyName?> GetPropertyNameByName(string name, Guid categoryId)
        {
            return await dbContext.PropertyNames.FirstOrDefaultAsync(x => x.PropertyName1 == name && x.CategoryId == categoryId);
        }
    }
}
