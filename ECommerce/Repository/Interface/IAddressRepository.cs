using ECommerce.Models.Domain;
using ECommerce.Models.DTOs;

namespace ECommerce.Repository.Interface
{
    public interface IAddressRepository
    {
        Task<List<Address>> GetAddressesByCustomerId(Guid customerId);
        Task AddAddress(Address address);
        void RemoveAddress(Address address);
        Task<Address?> GetAddressByAddressId(Guid addressId);
        Task<Address?> GetNonDefaultAddressOfCustomer(Guid customerId, Guid defaultAddressId);
    }
}
