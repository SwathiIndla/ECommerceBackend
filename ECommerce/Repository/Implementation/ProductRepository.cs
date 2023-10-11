using ECommerce.DbContext;
using ECommerce.Models.Domain;
using ECommerce.Models.DTOs;
using ECommerce.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Repository.Implementation
{
    public class ProductRepository : IProductRepository
    {
        private readonly EcommerceContext dbContext;

        public ProductRepository(EcommerceContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public List<ProductItemDetail> GetDistinctProductItems(List<ProductItemDetail> productItemsList)
        {
            return productItemsList.Distinct().ToList();
        }

        public async Task<ProductItemDetail?> GetProductItemById(Guid productItemId)
        {
            return await dbContext.ProductItemDetails.FirstOrDefaultAsync(item => item.ProductItemId == productItemId);
        }

        public List<ProductItemCardDto> GetProductItemsByPriceAsc(List<ProductItemCardDto> productItemCardsList)
        {
            return productItemCardsList.OrderBy(item => item.Price).ToList();
        }

        public List<ProductItemCardDto> GetProductItemsByPriceDesc(List<ProductItemCardDto> productItemCardsList)
        {
            return productItemCardsList.OrderByDescending(item => item.Price).ToList();
        }

        public List<ProductItemCardDto> GetProductItemsByRatingAsc(List<ProductItemCardDto> productItemCardsList)
        {
            return productItemCardsList.OrderBy(item => item.Rating).ToList();
        }

        public List<ProductItemCardDto> GetProductItemsByRatingDesc(List<ProductItemCardDto> productItemCardsList)
        {
            return productItemCardsList.OrderByDescending(item => item.Rating).ToList();
        }

        public async Task<List<ProductItemDetail>> GetSearchResults(string termToSearch)
        {
            return await dbContext.ProductItemDetails.Include(item => item.Product)
                    .Include(item => item.Product.ProductItemReviews)
                    .Include(item => item.SellerProductItems).ThenInclude(sellerProduct => sellerProduct.Seller)
                    .Include(item => item.ProductItemConfigurations).ThenInclude(config => config.PropertyValue).ThenInclude(propValue => propValue.PropertyName)
                    .Include(item => item.Product.Category).ThenInclude(category => category.PropertyNames)
                    .Where(item => item.ProductItemName.Replace(" ", string.Empty).ToLower().Contains(termToSearch.ToLower())).ToListAsync();
        }
    }
}
