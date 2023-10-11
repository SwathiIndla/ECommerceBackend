using ECommerce.DbContext;
using ECommerce.Models.Domain;
using ECommerce.Models.DTOs;
using ECommerce.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ECommerce.Repository.Implementation
{
    public class AddressRepository : IAddressRepository
    {
        private readonly EcommerceContext dbContext;

        public AddressRepository(EcommerceContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task AddAddress(Address address)
        {
            await dbContext.Addresses.AddAsync(address);
        }

        public async Task<List<Address>> GetAddressesByCustomerId(Guid customerId)
        {
            return await dbContext.Addresses.Where(address => address.CustomerId == customerId).ToListAsync();
        }

        public async Task<Address?> GetAddressByAddressId(Guid addressId)
        {
            return await dbContext.Addresses.Where(address => address.AddressId == addressId).FirstOrDefaultAsync();
        }

        public async Task<Address?> GetNonDefaultAddressOfCustomer(Guid customerId, Guid defaultAddressId)
        {
            return await dbContext.Addresses.FirstOrDefaultAsync(add => add.CustomerId == customerId && add.AddressId != defaultAddressId);
        }

        public void RemoveAddress(Address address)
        {
            dbContext.Addresses.Remove(address);
        }
    }
}
