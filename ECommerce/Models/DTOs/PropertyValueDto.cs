namespace ECommerce.Models.DTOs
{
    public class PropertyValueDto
    {
        public Guid PropertyValueId { get; set; }

        public Guid PropertyNameId { get; set; }

        public string PropertyValue { get; set; } = null!;
    }
}