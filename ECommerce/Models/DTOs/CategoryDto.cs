using ECommerce.Models.Domain;

namespace ECommerce.Models.DTOs
{
    public class CategoryDto
    {
        public Guid CategoryId { get; set; }

        public string CategoryName { get; set; } = null!;

        public Guid? ParentCategoryId { get; set; }

        public ICollection<CategoryDto> ChildCategories { get; set; } = new List<CategoryDto>();
    }
}
