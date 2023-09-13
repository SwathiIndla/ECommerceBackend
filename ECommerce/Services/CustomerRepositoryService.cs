using ECommerce.DbContext;
using ECommerce.Models.Domain;
using ECommerce.Repository;

namespace ECommerce.Services
{
    public class CustomerRepositoryService : ICustomerRepository
    {
        private readonly EcommerceContext dbContext;

        public CustomerRepositoryService(EcommerceContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<CustomerCredential> CreateCustomerAsync(CustomerCredential customerCredential)
        {
            await dbContext.CustomerCredentials.AddAsync(customerCredential);
            await dbContext.SaveChangesAsync();
            return customerCredential;
        }
    }
}
