using ECommerce.Models.Domain;

namespace ECommerce.Repository
{
    public interface ICategoriesRepository
    {
        Task<List<Category>> GetCategoryHierarchyAsync();
        Task<List<PropertyName>> GetPropertiesOfCategoryAsync(Guid categoryId);
    }
}
