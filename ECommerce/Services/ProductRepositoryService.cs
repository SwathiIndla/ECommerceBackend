using AutoMapper;
using ECommerce.DbContext;
using ECommerce.Models.Domain;
using ECommerce.Models.DTOs;
using ECommerce.Repository;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.Extensions.Msal;

namespace ECommerce.Services
{
    public class ProductRepositoryService : IProductRepository
    {
        private readonly EcommerceContext dbContext;
        private readonly IMapper mapper;
        private readonly int pageSize = 20;

        public ProductRepositoryService(EcommerceContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public async Task<Dictionary<Guid, List<ProductItemCardDto>>> SearchProductItem(string search)
        {
            var matchingProudcts = new List<ProductItemDetail>();
            var searchTerms = string.IsNullOrEmpty(search) ? Array.Empty<string>() : search.Split(' ');
            foreach (var term in searchTerms)
            {
                var results = await dbContext.ProductItemDetails
                    .Include(item => item.Product)
                    .Include(item => item.Product.ProductItemReviews)
                    .Include(item => item.ProductItemConfigurations)
                    .ThenInclude(config => config.PropertyValue)
                    .ThenInclude(propValue => propValue.PropertyName)
                    .Include(item => item.Product.Category)
                    .ThenInclude(category => category.PropertyNames)
                    .Where(item => item.ProductItemName.Replace(" ", string.Empty).ToLower().Contains(term.ToLower())).ToListAsync();
                matchingProudcts.AddRange(results);
            }
            var uniqueMatchingProducts = matchingProudcts.Distinct().ToList();
            var ListOfRawProductItemCardDto = mapper.Map<List<RawProductItemCardDto>>(uniqueMatchingProducts);
            var ListOfProductItemCardDto = mapper.Map<List<ProductItemCardDto>>(ListOfRawProductItemCardDto);
            var finalProductItemsData = ListOfProductItemCardDto.GroupBy(item => item.CategoryId).ToDictionary(k => k.Key, v => v.ToList());
            return finalProductItemsData;
        }

        public async Task<List<ProductItemCardDto>> FilterMobiles(FilterMobilesDto filterConditions)
        {
            var query = dbContext.ProductItemDetails.AsQueryable();
            //Filtering mobileproduct items on the brands
            query = filterConditions.Brands != null && filterConditions.Brands.Any() ? query.Where(item => filterConditions.Brands.Contains(item.Product.BrandId)) : query;
            //Filtering mobile product items on the colours
            query = filterConditions.Colour != null && filterConditions.Colour.Any() ? query.Where(item => item.ProductItemConfigurations.Any(
                config => config.PropertyValue.PropertyName.PropertyName1 == nameof(filterConditions.Colour) &&
                filterConditions.Colour.Contains(config.PropertyValue.PropertyValue1))) : query;
            //Filtering mobile product items on the RAM
            query = filterConditions.RAM != null && filterConditions.RAM.Any() ? query.Where(item => item.ProductItemConfigurations.Any(
                config => config.PropertyValue.PropertyName.PropertyName1 == nameof(filterConditions.RAM) &&
                filterConditions.RAM.Contains(config.PropertyValue.PropertyValue1))) : query;
            //Filtering mobile product items on the storage
            query = filterConditions.Storage != null && filterConditions.Storage.Any() ? query.Where(item => item.ProductItemConfigurations.Any(
                config => config.PropertyValue.PropertyName.PropertyName1 == nameof(filterConditions.Storage) && 
                filterConditions.Storage.Contains(config.PropertyValue.PropertyValue1))) : query;
            //Filtering mobile product items on the battery
            query = filterConditions.Battery != null && filterConditions.Battery.Any() ? query.Where(item => item.ProductItemConfigurations.Any(
                config => config.PropertyValue.PropertyName.PropertyName1 == nameof(filterConditions.Battery) && 
                filterConditions.Battery.Contains(config.PropertyValue.PropertyValue1))) : query;
            //Filtering mobile product items on the screen size
            query = filterConditions.Screen_Size != null && filterConditions.Screen_Size.Any() ? query.Where(item => item.ProductItemConfigurations.Any(
                config => config.PropertyValue.PropertyName.PropertyName1 == nameof(filterConditions.Screen_Size) && 
                filterConditions.Screen_Size.Contains(config.PropertyValue.PropertyValue1))) : query;
            //Filtering mobile product items on the resolution
            query = filterConditions.Resolution != null && filterConditions.Resolution.Any() ? query.Where(item => item.ProductItemConfigurations.Any(
                config => config.PropertyValue.PropertyName.PropertyName1 == nameof(filterConditions.Resolution) && 
                filterConditions.Resolution.Contains(config.PropertyValue.PropertyValue1))) : query;
            //Filtering mobile product items on the primary camera
            query = filterConditions.Primary_Camera != null && filterConditions.Primary_Camera.Any() ? query.Where(item => item.ProductItemConfigurations.Any(
                config => config.PropertyValue.PropertyName.PropertyName1 == nameof(filterConditions.Primary_Camera) &&
                filterConditions.Primary_Camera.Contains(config.PropertyValue.PropertyValue1))) : query;
            //Filtering mobile product items on the secondary camera
            query = filterConditions.Secondary_Camera != null && filterConditions.Secondary_Camera.Any() ? query.Where(item => item.ProductItemConfigurations.Any(
                config => config.PropertyValue.PropertyName.PropertyName1 == nameof(filterConditions.Secondary_Camera) &&
                filterConditions.Secondary_Camera.Contains(config.PropertyValue.PropertyValue1))) : query;
            //Filtering mobile product items on the processor
            query = filterConditions.Processor != null && filterConditions.Processor.Any() ? query.Where(item => item.ProductItemConfigurations.Any(
                config => config.PropertyValue.PropertyName.PropertyName1 == nameof(filterConditions.Processor) &&
                filterConditions.Processor.Contains(config.PropertyValue.PropertyValue1))) : query;

            query = filterConditions.MinPrice.HasValue ? query.Where(item => item.Price >= filterConditions.MinPrice.Value) : query;

            query = filterConditions.MaxPrice.HasValue ? query.Where(item => item.Price <= filterConditions.MaxPrice.Value) : query;
            query = query.Include(item => item.Product)
                    .Include(item => item.Product.ProductItemReviews)
                    .Include(item => item.ProductItemConfigurations)
                    .ThenInclude(config => config.PropertyValue)
                    .ThenInclude(propValue => propValue.PropertyName)
                    .Include(item => item.Product.Category)
                    .ThenInclude(category => category.PropertyNames);
            if (filterConditions.SortOnPrice)
            {
                query = filterConditions.SortByPriceAsc ? query.OrderBy(item => item.Price) : query.OrderByDescending(item => item.Price);
            }
            if (filterConditions.SortOnRating)
            {
                query = query.OrderByDescending(item => item.Product.ProductItemReviews.Average(rating => rating.Rating));
            }
            var filteredResults = await query.Skip((filterConditions.Page - 1)*pageSize).Take(pageSize).ToListAsync();
            var rawProductItemDtoCard = mapper.Map<List<RawProductItemCardDto>>(filteredResults);
            var productItemDtoCard = mapper.Map<List<ProductItemCardDto>>(rawProductItemDtoCard);
            
            return productItemDtoCard;
        }

        public Task<object> GetDetailedProductItem(Guid productItemId)
        {
            throw new NotImplementedException();
        }
    }
}
