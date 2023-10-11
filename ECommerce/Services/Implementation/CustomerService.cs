using AutoMapper;
using ECommerce.DbContext;
using ECommerce.Models.Domain;
using ECommerce.Models.DTOs;
using ECommerce.Repository.Interface;
using ECommerce.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Services.Implementation
{
    public class CustomerService : ICustomerService
    {
        private readonly IMapper mapper;
        private readonly IAddressRepository addressRepository;
        private readonly ICustomerRepository customerRepository;
        private readonly ISaveChangesRepository saveChangesRepository;

        public CustomerService(IMapper mapper, IAddressRepository addressRepository, ICustomerRepository customerRepository,
            ISaveChangesRepository saveChangesRepository)
        {
            this.mapper = mapper;
            this.addressRepository = addressRepository;
            this.customerRepository = customerRepository;
            this.saveChangesRepository = saveChangesRepository;
        }

        public async Task<AddressDto> AddAddressToCustomer(AddAddressRequestDto addressDto)
        {
            var user = await customerRepository.GetCustomerById(addressDto.CustomerId);
            Address? addressDomain = null;
            if (user != null)
            {
                var ListOfAddresses = await addressRepository.GetAddressesByCustomerId(addressDto.CustomerId);
                addressDomain = mapper.Map<Address>(addressDto);
                addressDomain.AddressId = Guid.NewGuid();
                if (ListOfAddresses.Count == 0)
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
                await addressRepository.AddAddress(addressDomain);
                await saveChangesRepository.AsynchronousSaveChanges();
            }
            var newAddress = mapper.Map<AddressDto>(addressDomain);
            return newAddress;
        }

        public async Task<CustomerCredential> CreateCustomerAsync(CustomerCredential customerCredential)
        {
            await customerRepository.AddCustomer(customerCredential);
            await saveChangesRepository.AsynchronousSaveChanges();
            return customerCredential;
        }

        public async Task<List<AddressDto>> GetAddressesOfCustomer(Guid customerId)
        {
            var addresses = await addressRepository.GetAddressesByCustomerId(customerId);
            var addressesDto = mapper.Map<List<AddressDto>>(addresses);
            return addressesDto;
        }

        public async Task<AddressDto?> UpdateAddress(AddressDto addressDto)
        {
            var address = await addressRepository.GetAddressByAddressId(addressDto.AddressId);
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
            await saveChangesRepository.AsynchronousSaveChanges();
            var updatedAddress = mapper.Map<AddressDto>(address);
            return updatedAddress;
        }

        public async Task<bool> SetDefaultAddress(Guid addressId)
        {
            var address = await addressRepository.GetAddressByAddressId(addressId);
            var success = false;
            if (address != null)
            {
                address.IsDefault = true; success = true;
                var addresses = await addressRepository.GetAddressesByCustomerId(address.CustomerId);
                foreach (var add in addresses)
                {
                    if (add.AddressId != addressId) add.IsDefault = false;
                }
            }
            await saveChangesRepository.AsynchronousSaveChanges();
            return success;
        }

        public async Task<bool> DeleteAddress(Guid addressId)
        {
            var address = await addressRepository.GetAddressByAddressId(addressId);
            var success = false;
            if (address != null)
            {
                if (address.IsDefault)
                {
                    var newDefaultAddress = await addressRepository.GetNonDefaultAddressOfCustomer(address.CustomerId, address.AddressId);
                    if (newDefaultAddress != null) newDefaultAddress.IsDefault = true;
                }
                addressRepository.RemoveAddress(address);
                await saveChangesRepository.AsynchronousSaveChanges();
                success = true;
            }
            return success;
        }

        public async Task<Cart> CreateCartAsync(Cart customerCart)
        {
            await customerRepository.CreateCustomerCart(customerCart);
            await saveChangesRepository.AsynchronousSaveChanges();
            return customerCart;
        }

        public async Task<CustomerCredential?> GetCustomerById(Guid customerId)
        {
            return await customerRepository.GetCustomerById(customerId);
        }
    }
}
