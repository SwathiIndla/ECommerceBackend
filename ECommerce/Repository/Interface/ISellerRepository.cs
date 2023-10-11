using ECommerce.Models.Domain;

namespace ECommerce.Repository.Interface
{
    public interface ISellerRepository
    {
        Task<Seller?> GetSellerAsync(Guid sellerId);
    }
}
