using AutoFixture;
using AutoMapper;
using ECommerce.DbContext;
using ECommerce.Models.Domain;
using ECommerce.Models.DTOs;
using ECommerce.Repository.Interface;
using ECommerce.Services.Implementation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.UnitTests.Services
{
    public class CustomerRepositoryServiceTests
    {
        private readonly IFixture fixture;
        private readonly Mock<IMapper> mapperMock;
        private readonly Mock<IAddressRepository> addressRepositoryMock;
        private readonly Mock<ICustomerRepository> customerRepositoryMock;
        private readonly Mock<ISaveChangesRepository> saveChangesRepositoryMock;
        private readonly CustomerService sut;

        public CustomerRepositoryServiceTests()
        {
            fixture = new Fixture();
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior(1));
            addressRepositoryMock = new Mock<IAddressRepository>();
            customerRepositoryMock = new Mock<ICustomerRepository>();
            saveChangesRepositoryMock = new Mock<ISaveChangesRepository>();
            mapperMock = new Mock<IMapper>();
            sut = new CustomerService(mapperMock.Object, addressRepositoryMock.Object, customerRepositoryMock.Object
                , saveChangesRepositoryMock.Object);
        }

        [Fact]
        public async Task AddAddressToCustomer_ShouldReturnAddressDto_WhenAddressAddedSuccessfully()
        {
            var user = fixture.Create<CustomerCredential>();
            var addressRequestDto = fixture.Create<AddAddressRequestDto>();
            var addressDomain = fixture.Create<Address>();
            var addressDto = fixture.Create<AddressDto>();
            var addressList = fixture.Create<List<Address>>();

            customerRepositoryMock.Setup(x => x.GetCustomerById(It.IsAny<Guid>())).ReturnsAsync(user);
            addressRepositoryMock.Setup(x => x.GetAddressesByCustomerId(It.IsAny<Guid>())).ReturnsAsync(addressList);

            mapperMock.Setup(mapper => mapper.Map<Address>(addressRequestDto)).Returns(addressDomain);
            mapperMock.Setup(mapper => mapper.Map<AddressDto>(addressDomain)).Returns(addressDto);

            var result = await sut.AddAddressToCustomer(addressRequestDto);

            Assert.NotNull(result);
            Assert.IsType<AddressDto>(result);
            Assert.Equal(addressDto.ToString(), result.ToString());
        }

        [Fact]
        public async Task CreateCustomerAsync_ShouldReturnOkResponse_WhenValidDetailsProvided()
        {
            var customerCredentials = fixture.Create<CustomerCredential>();

            var result = await sut.CreateCustomerAsync(customerCredentials).ConfigureAwait(false);

            Assert.NotNull(result);
            Assert.IsType<CustomerCredential>(result);
            Assert.Equal(customerCredentials.ToString(), result.ToString());
        }
    }
}
