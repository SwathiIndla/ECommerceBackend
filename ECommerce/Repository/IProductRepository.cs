using ECommerce.Models.Domain;
using ECommerce.Models.DTOs;

namespace ECommerce.Repository
{
    public interface IProductRepository
    {
        Task<PaginatedSearchResultsDto> SearchProductItem(string search, int page, SortProductsDto sortConditions);
        Task<PaginatedFilterResults> FilterProducts(Guid categoryId, FilterProductsQueryParametersDto filterConditions, SortProductsDto sortConditions);
        Task<ProductItemDetailedPageDto?> GetDetailedProductItem(Guid productItemId);
        Task<ProductVariantDetailedPageDto> FilterProductVariant(Guid productId, FilterVariantParametersDto filterConditions, List<string> featuresDataNeeded);
    }
}
