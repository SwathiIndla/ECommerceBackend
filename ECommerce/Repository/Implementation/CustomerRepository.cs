using ECommerce.DbContext;
using ECommerce.Models.Domain;
using ECommerce.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Repository.Implementation
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly EcommerceContext dbContext;

        public CustomerRepository(EcommerceContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task AddCustomer(CustomerCredential customerCredential)
        {
            await dbContext.CustomerCredentials.AddAsync(customerCredential);
        }

        public async Task CreateCustomerCart(Cart customerCart)
        {
            await dbContext.Carts.AddAsync(customerCart);
        }

        public async Task<CustomerCredential?> GetCustomerById(Guid customerId)
        {
            return await dbContext.CustomerCredentials.FirstOrDefaultAsync(customer => customer.CustomerId == customerId);
        }
    }
}
