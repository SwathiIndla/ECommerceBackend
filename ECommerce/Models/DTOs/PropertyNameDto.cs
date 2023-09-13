using System.ComponentModel.DataAnnotations;

namespace ECommerce.Models.DTOs
{
    public class PropertyNameDto
    {
        public Guid PropertyId { get; set; }

        public Guid CategoryId { get; set; }

        public string PropertyName { get; set; } = null!;
    }
}
