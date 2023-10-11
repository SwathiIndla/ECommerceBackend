using ECommerce.Models.Domain;
using ECommerce.Models.DTOs;

namespace ECommerce.Repository.Interface
{
    public interface IProductRepository
    {
        Task<ProductItemDetail?> GetProductItemById(Guid productItemId);
        Task<List<ProductItemDetail>> GetSearchResults(string termToSearch);
        List<ProductItemDetail> GetDistinctProductItems(List<ProductItemDetail> productItemsList);
        List<ProductItemCardDto> GetProductItemsByPriceAsc(List<ProductItemCardDto> productItemCardsList);
        List<ProductItemCardDto> GetProductItemsByPriceDesc(List<ProductItemCardDto> productItemCardsList);
        List<ProductItemCardDto> GetProductItemsByRatingAsc(List<ProductItemCardDto> productItemCardsList);
        List<ProductItemCardDto> GetProductItemsByRatingDesc(List<ProductItemCardDto> productItemCardsList);
    }
}
