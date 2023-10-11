using ECommerce.Models.Domain;

namespace ECommerce.Repository.Interface
{
    public interface ICategoriesRepository
    {
        Task<List<Brand>> GetBrandsOfCategory(Guid categoryId);
        Task<List<Category>> GetCategoryHierarchy();
        Task<List<PropertyName>> GetPropertiesOfCategory(Guid categoryId);
    }
}
