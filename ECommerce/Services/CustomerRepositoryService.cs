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
            var user = await dbContext.CustomerCredentials.FirstOrDefaultAsync(customer => customer.CustomerId == addressDto.CustomerId);
            Address? addressDomain = null;
            if (user != null)
            {
                var ListOfAddresses = dbContext.Addresses.Where(address => address.CustomerId == addressDto.CustomerId);
                addressDomain = mapper.Map<Address>(addressDto);
                addressDomain.AddressId = Guid.NewGuid();
                if (ListOfAddresses.Count() == 0)
                {
                    addressDomain.IsDefault = true;
                }
                else
                {
                    if (addressDto.IsDefault)
                    {
                        foreach (var Address in ListOfAddresses)
                        {
                            Address.IsDefault = false;
                        }
                    }
                }
                await dbContext.Addresses.AddAsync(addressDomain);
                await dbContext.SaveChangesAsync();
            }
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
            }
            await dbContext.SaveChangesAsync();
            var updatedAddress = mapper.Map<AddressDto>(address);
            return updatedAddress;
        }

        public async Task<bool> SetDefaultAddress(Guid addressId)
        {
            var address = await dbContext.Addresses.FirstOrDefaultAsync(add => add.AddressId == addressId);
            var success = false;
            if (address != null)
            {
                address.IsDefault = true; success = true;
                var addresses = dbContext.Addresses.Where(add => add.CustomerId == address.CustomerId && add.AddressId != addressId).AsEnumerable();
                foreach (var add in addresses)
                {
                    add.IsDefault = false;
                }
            }
            await dbContext.SaveChangesAsync();
            return success;
        }

        public async Task<bool> DeleteAddress(Guid addressId)
        {
            var address = await dbContext.Addresses.FirstOrDefaultAsync(address => address.AddressId == addressId);
            var success = false;
            if (address != null)
            {
                if (address.IsDefault)
                {
                    var newDefaultAddress = await dbContext.Addresses.FirstOrDefaultAsync(add => add.CustomerId == address.CustomerId && add.AddressId != address.AddressId);
                    if (newDefaultAddress != null) newDefaultAddress.IsDefault = true;
                }
                dbContext.Addresses.Remove(address);
                await dbContext.SaveChangesAsync();
                success = true;
            }
            return success;
        }

        public async Task<Cart> CreateCartAsync(Cart customerCart)
        {
            await dbContext.Carts.AddAsync(customerCart);
            await dbContext.SaveChangesAsync();
            return customerCart;
        }

        public async Task<CustomerCredential?> GetCustomerById(Guid customerId)
        {
            return await dbContext.CustomerCredentials.FirstOrDefaultAsync(customer => customer.CustomerId == customerId);
        }
    }
}
