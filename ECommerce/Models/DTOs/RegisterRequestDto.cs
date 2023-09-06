using System.ComponentModel.DataAnnotations;

namespace ECommerce.Models.DTOs
{
    public class RegisterRequestDto
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        public string[] Roles { get; set; } = null!;
    }
}
