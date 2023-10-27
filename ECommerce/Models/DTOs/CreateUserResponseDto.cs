namespace ECommerce.Models.DTOs
{
    public class CreateUserResponseDto
    {
        public bool succeeded { get; set; }
        public Guid customerId { get; set; }
        public string email { get; set; } = null!;
        public string passwordHash { get; set; } = null!;
    }
}
