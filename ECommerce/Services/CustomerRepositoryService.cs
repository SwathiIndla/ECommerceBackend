using AutoMapper;
using ECommerce.DbContext;
using ECommerce.Models.Domain;
using ECommerce.Models.DTOs;
using ECommerce.Repository;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Services
{
    public class CustomerRepositoryService : ICustomerRepository
    {
        private readonly EcommerceContext dbContext;
        private readonly IMapper mapper;

        public CustomerRepositoryService(EcommerceContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public async Task<AddressDto> AddAddressToCustomer(AddAddressRequestDto addressDto)
        {
            var addressDomain = mapper.Map<Address>(addressDto);
            await dbContext.Addresses.AddAsync(addressDomain);
            await dbContext.SaveChangesAsync();
            var newAddress = mapper.Map<AddressDto>(addressDomain);
            return newAddress;
        }

        public async Task<CustomerCredential> CreateCustomerAsync(CustomerCredential customerCredential)
        {
            await dbContext.CustomerCredentials.AddAsync(customerCredential);
            await dbContext.SaveChangesAsync();
            return customerCredential;
        }

        public async Task<List<AddressDto>> GetAddressesOfCustomer(Guid customerId)
        {
            var addresses = await dbContext.Addresses.Where(address => address.CustomerId == customerId).ToListAsync();
            var addressesDto = mapper.Map<List<AddressDto>>(addresses);
            return addressesDto;
        }

        public async Task<AddressDto?> UpdateAddress(AddressDto addressDto)
        {
            var address = await dbContext.Addresses.Where(address => address.AddressId == addressDto.AddressId).FirstOrDefaultAsync();
            if (address != null)
            {
                address.CustomerName = addressDto.CustomerName;
                address.PhoneNumber = addressDto.PhoneNumber;
                address.StreetAddress = addressDto.StreetAddress;
                address.City = addressDto.City;
                address.StateProvince = addressDto.StateProvince;
                address.Country = addressDto.Country;
                address.PostalCode = addressDto.PostalCode;
                address.AddressType = addressDto.AddressType;
                address.IsDefault = addressDto.IsDefault;
            }
            await dbContext.SaveChangesAsync();
            var updatedAddress = mapper.Map<AddressDto>(address);
            return updatedAddress;
        }
    }
}
