namespace ECommerce.Models.DTOs
{
    public class UserCreationJson
    {
        public bool succeeded { get; set; }
        public List<Error> errors { get; set; } = new List<Error>();
    }
}
