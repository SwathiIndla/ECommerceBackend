using ECommerce.DbContext;
using ECommerce.Models.Domain;
using ECommerce.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Repository.Implementation
{
    public class CategoriesRepository : ICategoriesRepository
    {
        private readonly EcommerceContext dbContext;

        public CategoriesRepository(EcommerceContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Brand>> GetBrandsOfCategory(Guid categoryId)
        {
            return await dbContext.BrandCategories.Where(brandCategory => brandCategory.CategoryId == categoryId).Select(brand => brand.Brand).ToListAsync();
        }

        public async Task<List<Category>> GetCategoryHierarchy()
        {
            return await dbContext.Categories.Where(x => x.ParentCategoryId == null).Include(x => x.InverseParentCategory).ToListAsync();
        }

        public async Task<List<PropertyName>> GetPropertiesOfCategory(Guid categoryId)
        {
            return await dbContext.PropertyNames.Where(x => x.CategoryId == categoryId).Include(x => x.PropertyValues).ToListAsync();
        }
    }
}
