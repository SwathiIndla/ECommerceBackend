using ECommerce.Models.Domain;
using ECommerce.Models.DTOs;

namespace ECommerce.Services.Interface
{
    public interface ICustomerService
    {
        Task<CustomerCredential> CreateCustomerAsync(CustomerCredential customerCredential);
        Task<AddressDto> AddAddressToCustomer(AddAddressRequestDto addressDto);
        Task<List<AddressDto>> GetAddressesOfCustomer(Guid customerId);
        Task<AddressDto?> UpdateAddress(AddressDto addressDto);
        Task<bool> SetDefaultAddress(Guid addressId);
        Task<bool> DeleteAddress(Guid addressId);
        Task<Cart> CreateCartAsync(Cart customerCart);
        Task<CustomerCredential?> GetCustomerById(Guid customerId);
    }
}
