using ECommerce.Models.Domain;

namespace ECommerce.Repository
{
    public interface ICustomerRepository
    {
        Task<CustomerCredential> CreateCustomerAsync(CustomerCredential customerCredential);
    }
}
