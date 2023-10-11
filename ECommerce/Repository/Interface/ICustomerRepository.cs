using ECommerce.Models.Domain;

namespace ECommerce.Repository.Interface
{
    public interface ICustomerRepository
    {
        Task<CustomerCredential?> GetCustomerById(Guid customerId);
        Task AddCustomer(CustomerCredential customerCredential);
        Task CreateCustomerCart(Cart customerCart);
    }
}
