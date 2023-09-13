using ECommerce.DbContext;
using ECommerce.Models.Domain;
using ECommerce.Repository;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Services
{
    public class CategoriesRepositoryService : ICategoriesRepository
    {
        private readonly EcommerceContext dbContext;

        public CategoriesRepositoryService(EcommerceContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Category>> GetCategoryHierarchyAsync()
        {
            return await dbContext.Categories.Where(x => x.ParentCategoryId == null).Include(x => x.InverseParentCategory).ToListAsync();
        }

        public async Task<List<PropertyName>> GetPropertiesOfCategoryAsync(Guid categoryId)
        {
            return await dbContext.PropertyNames.Where(x => x.CategoryId == categoryId).Include(x => x.PropertyValues).ToListAsync();
        }
    }
}
