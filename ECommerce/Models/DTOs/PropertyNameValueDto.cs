namespace ECommerce.Models.DTOs
{
    public class PropertyNameValueDto
    {
        public Guid PropertyId { get; set; }

        public Guid CategoryId { get; set; }

        public string PropertyName { get; set; } = null!;

        public List<PropertyValueDto> PropertyValues { get; set; } = new List<PropertyValueDto>();
    }
}
