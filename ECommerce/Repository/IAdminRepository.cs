using ECommerce.Models.Domain;

namespace ECommerce.Repository
{
    public interface IAdminRepository
    {
        Task<Category> AddCategory(Category category);
        Task<BrandCategory> AddBrand(Brand brand, Category category);
        Task<Category?> GetCategoryByName(string name);
        Task<Brand?> GetBrandByName(string name);
        Task<PropertyName> AddPropertyName(string name, Category category);
        Task<PropertyName?> GetPropertyNameByName(string name, Guid categoryId);
    }
}
