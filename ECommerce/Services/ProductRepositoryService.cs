using ECommerce.DbContext;
using ECommerce.Models.Domain;
using ECommerce.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.Extensions.Msal;

namespace ECommerce.Services
{
    public class ProductRepositoryService : IProductRepository
    {
        private readonly EcommerceContext dbContext;

        public ProductRepositoryService(EcommerceContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IQueryable<ProductItemDetail> SearchProductItem(IQueryable<ProductItemDetail> query, string? search)
        {
            return string.IsNullOrEmpty(search) ? query : query.Where(item => item.ProductItemName.ToLower().Contains(search.ToLower()));
        }

        public IQueryable<ProductItemDetail> FilterProductItemOnBrand(IQueryable<ProductItemDetail> query, List<Guid>? brands)
        {
            return brands != null && brands.Count > 0 ? query.Where(item => brands.Contains(item.Product.BrandId)) : query;
        }

        public object FetchMobileProductItems(string? search = null, List<Guid>? brands = null, List<string>? Colour = null, List<string>? RAM = null, List<string>? Storage = null, List<string>? Battery = null, List<string>? Screen_Size = null, List<string>? Resolution = null, List<string>? Primary_Camera = null, List<string>? Secondary_Camera = null, decimal? minPrice = null, decimal? maxPrice = null, int page = 1, bool sortOnPrice = false, bool sortByPriceAsc = false, bool sortOnRating = true, bool sortByRatingAsc = false)
        {
            int pageSize = 10;
            var query = dbContext.ProductItemDetails.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(item => item.ProductItemName.ToLower().Contains(search.ToLower()));
            }
            if (Colour != null && Colour.Any())
            {
                query = query.Where(item => item.ProductItemConfigurations.Any(
                    config => config.PropertyValue.PropertyName.PropertyName1 == "Colour" &&
                    Colour.Contains(config.PropertyValue.PropertyValue1)));
            }
            if (RAM != null && RAM.Any())
            {
                query = query.Where(item => item.ProductItemConfigurations.Any(
                    config => config.PropertyValue.PropertyName.PropertyName1 == "RAM" &&
                    RAM.Contains(config.PropertyValue.PropertyValue1)));
            }
            if (Storage != null && Storage.Any())
            {
                query = query.Where(item => item.ProductItemConfigurations.Any(
                    config => config.PropertyValue.PropertyName.PropertyName1 == "Storage" &&
                    Storage.Contains(config.PropertyValue.PropertyValue1)));
            }
            if (Battery != null && Battery.Any())
            {
                query = query.Where(item => item.ProductItemConfigurations.Any(
                    config => config.PropertyValue.PropertyName.PropertyName1 == "Battery" &&
                    Battery.Contains(config.PropertyValue.PropertyValue1)));
            }
            if (Screen_Size != null && Screen_Size.Any())
            {
                query = query.Where(item => item.ProductItemConfigurations.Any(
                    config => config.PropertyValue.PropertyName.PropertyName1 == "Screen_Size" &&
                    Screen_Size.Contains(config.PropertyValue.PropertyValue1)));
            }
            if (Resolution != null && Resolution.Any())
            {
                query = query.Where(item => item.ProductItemConfigurations.Any(
                    config => config.PropertyValue.PropertyName.PropertyName1 == "Resolution" &&
                    Resolution.Contains(config.PropertyValue.PropertyValue1)));
            }
            if (Primary_Camera != null && Primary_Camera.Any())
            {
                query = query.Where(item => item.ProductItemConfigurations.Any(
                    config => config.PropertyValue.PropertyName.PropertyName1 == "Primary_Camera" &&
                    Primary_Camera.Contains(config.PropertyValue.PropertyValue1)));
            }
            if (Secondary_Camera != null && Secondary_Camera.Any())
            {
                query = query.Where(item => item.ProductItemConfigurations.Any(
                    config => config.PropertyValue.PropertyName.PropertyName1 == "Secondary_Camera" &&
                    Secondary_Camera.Contains(config.PropertyValue.PropertyValue1)));
            }
            if (minPrice.HasValue)
            {
                query = query.Where(item => item.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(item => item.Price <= maxPrice.Value);
            }
            var initialDetails = query.Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(
                item => new
                {
                    item.ProductItemId,
                    item.ProductId,
                    item.QtyInStock,
                    item.Sku,
                    item.Price,
                    item.ProductItemImage,
                    Properties = dbContext.PropertyNames
                    .Where(propName => propName.CategoryId == item.Product.CategoryId)
                    .Select(propName => new
                    {
                        Name = propName.PropertyName1,
                        Value = item.ProductItemConfigurations
                        .Where(config => config.PropertyValue.PropertyName.PropertyId == propName.PropertyId)
                        .Select(config => config.PropertyValue.PropertyValue1)
                        .FirstOrDefault()
                    }).AsEnumerable()
                }).AsEnumerable();
            var result = initialDetails.Select(item => new
            {
                item.ProductItemId,
                item.ProductId,
                item.QtyInStock,
                item.Sku,
                item.Price,
                item.ProductItemImage,
                specifications = item.Properties.ToDictionary(prop => prop.Name, prop => prop.Value)
            }).ToList();
            return result;
        }
    }
}
