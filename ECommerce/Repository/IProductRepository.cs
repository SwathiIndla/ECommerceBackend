using ECommerce.Models.Domain;
using ECommerce.Models.DTOs;

namespace ECommerce.Repository
{
    public interface IProductRepository
    {
        Task<Dictionary<Guid, List<ProductItemCardDto>>> SearchProductItem(string search);
        Task<List<ProductItemCardDto>> FilterMobiles(FilterMobilesQueryParameters filterConditions);
        Task<object> GetDetailedProductItem(Guid productItemId);
    }
}
