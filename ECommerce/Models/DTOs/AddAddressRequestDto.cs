namespace ECommerce.Models.DTOs
{
    public class AddAddressRequestDto
    {
        public string CustomerName { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public string StreetAddress { get; set; } = null!;

        public string City { get; set; } = null!;

        public string StateProvince { get; set; } = null!;

        public string Country { get; set; } = null!;

        public string PostalCode { get; set; } = null!;

        public string AddressType { get; set; } = null!;

        public bool IsDefault { get; set; } = false;

        public Guid CustomerId { get; set; }
    }
}
