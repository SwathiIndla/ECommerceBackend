using ECommerce.Models.Domain;

namespace ECommerce.Services.Interface
{
    public interface ICategoriesService
    {
        Task<List<Category>> GetCategoryHierarchyAsync();
        Task<List<PropertyName>> GetPropertiesOfCategoryAsync(Guid categoryId);
        Task<List<Brand>> GetBrandsAsync(Guid categoryId);
    }
}
