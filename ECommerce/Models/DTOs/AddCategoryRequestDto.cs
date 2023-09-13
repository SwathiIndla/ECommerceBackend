using System.ComponentModel.DataAnnotations;

namespace ECommerce.Models.DTOs
{
    public class AddCategoryRequestDto
    {
        [Required]
        public string CategoryName { get; set; } = null!;

        public Guid? ParentCategoryId { get; set; }
    }
}
