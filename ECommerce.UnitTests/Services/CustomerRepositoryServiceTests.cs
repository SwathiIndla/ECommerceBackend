using AutoFixture;
using AutoMapper;
using ECommerce.DbContext;
using ECommerce.Models.Domain;
using ECommerce.Models.DTOs;
using ECommerce.Services;
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
        private readonly Mock<EcommerceContext> contextMock;
        private readonly Mock<IMapper> mapperMock;
        private readonly CustomerRepositoryService sut;

        public CustomerRepositoryServiceTests()
        {
            fixture = new Fixture();
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior(1));
            contextMock = new Mock<EcommerceContext>();
            mapperMock = new Mock<IMapper>();
            sut = new CustomerRepositoryService(contextMock.Object, mapperMock.Object);
        }

        //[Fact]
        public async Task AddAddressToCustomer_ShouldReturnAddressDto_WhenAddressAddedSuccessfully()
        {
            var customerDataMock = new Mock<DbSet<CustomerCredential>>();
            var user = fixture.Create<CustomerCredential>();
            var addressDataMock = new Mock<DbSet<Address>>();
            var addressRequestDto = fixture.Create<AddAddressRequestDto>();
            var addressDomain = fixture.Create<Address>();
            var addressDto = fixture.Create<AddressDto>();
            var addressList = fixture.Create<List<Address>>();
            var addressListQueryable = new List<Address>().AsQueryable();

            contextMock.Setup(context => context.Set<CustomerCredential>()).Returns(customerDataMock.Object);
            contextMock.Setup(context => context.Set<Address>()).Returns(addressDataMock.Object);
            contextMock.Setup(context => context.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

            //customerDataMock.Setup(customer => customer.FirstOrDefaultAsync(It.IsAny<Expression<Func<CustomerCredential, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(user);
            //addressDataMock.Setup(address => address.Where(It.IsAny<Expression<Func<Address, bool>>>())).Returns(addressListQueryable);
            addressDataMock.Setup(address => address.AddAsync(It.IsAny<Address>(), It.IsAny<CancellationToken>())).Callback((Address model, CancellationToken token) => { addressList.Add(model); })
                .Returns((Address model, CancellationToken token) => ValueTask.FromResult((EntityEntry<Address>?) null));

            mapperMock.Setup(mapper => mapper.Map<Address>(addressRequestDto)).Returns(addressDomain);
            mapperMock.Setup(mapper => mapper.Map<AddressDto>(addressDomain)).Returns(addressDto);

            var result = await sut.AddAddressToCustomer(addressRequestDto);

            Assert.NotNull(result);
        }
    }
}
