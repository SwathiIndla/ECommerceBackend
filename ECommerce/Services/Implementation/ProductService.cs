using AutoMapper;
using ECommerce.DbContext;
using ECommerce.Models.Domain;
using ECommerce.Models.DTOs;
using ECommerce.Repository.Interface;
using ECommerce.Services.Interface;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Identity.Client.Extensions.Msal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ECommerce.Services.Implementation
{
    public class ProductService : IProductService
    {
        private readonly EcommerceContext dbContext;
        private readonly IMapper mapper;
        private readonly IProductRepository productRepository;
        private readonly int pageSize = 20;

        public ProductService(EcommerceContext dbContext, IMapper mapper, IProductRepository productRepository)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.productRepository = productRepository;
        }

        public async Task<PaginatedSearchResultsDto> SearchProductItem(string search, int page, SortProductsDto sortConditions)
        {
            var matchingProducts = new List<ProductItemDetail>();
            var searchTerms = string.IsNullOrEmpty(search) ? Array.Empty<string>() : search.Split(' ');
            foreach (var term in searchTerms)
            {
                var results = await productRepository.GetSearchResults(term);
                matchingProducts.AddRange(results);
            }
            var uniqueMatchingProducts = productRepository.GetDistinctProductItems(matchingProducts);
            var ListOfProductItemCardDto = mapper.Map<List<ProductItemCardDto>>(uniqueMatchingProducts);
            if (sortConditions.SortOnPrice)
            {
                ListOfProductItemCardDto = sortConditions.SortByPriceAsc ? productRepository.GetProductItemsByPriceAsc(ListOfProductItemCardDto) : productRepository.GetProductItemsByPriceDesc(ListOfProductItemCardDto);
            }
            if (sortConditions.SortOnRating)
            {
                ListOfProductItemCardDto = sortConditions.SortByRatingAsc ? productRepository.GetProductItemsByRatingAsc(ListOfProductItemCardDto) : productRepository.GetProductItemsByRatingDesc(ListOfProductItemCardDto);
            }
            var finalSearchResult = new PaginatedSearchResultsDto
            {
                MultipleCategories = ListOfProductItemCardDto.GroupBy(item => item.CategoryId).ToList().Count > 1,
                TotalSearchResults = ListOfProductItemCardDto.Count,
                SearchResults = ListOfProductItemCardDto.Skip((page - 1) * pageSize).Take(pageSize).ToList()
            };
            return finalSearchResult;
        }

        public async Task<PaginatedFilterResults> FilterProducts(Guid categoryId, FilterProductsQueryParametersDto filterConditions, SortProductsDto sortConditions)
        {
            var query = dbContext.ProductItemDetails.Include(item => item.Product).Where(item => item.Product.CategoryId == categoryId).AsQueryable();
            //Filtering product items on the brands
            query = filterConditions.Brands != null && filterConditions.Brands.Any() ? query.Where(item => filterConditions.Brands.Contains(item.Product.BrandId)) : query;
            //Filtering product items on the colours
            query = filterConditions.Colour != null && filterConditions.Colour.Any() ? query.Where(item => item.ProductItemConfigurations.Any(
                config => config.PropertyValue.PropertyName.PropertyName1 == nameof(filterConditions.Colour) &&
                filterConditions.Colour.Contains(config.PropertyValue.PropertyValue1))) : query;
            //Filtering product items on the RAM
            query = filterConditions.RAM != null && filterConditions.RAM.Any() ? query.Where(item => item.ProductItemConfigurations.Any(
                config => config.PropertyValue.PropertyName.PropertyName1 == nameof(filterConditions.RAM) &&
                filterConditions.RAM.Contains(config.PropertyValue.PropertyValue1))) : query;
            //Filtering product items on the storage
            query = filterConditions.Storage != null && filterConditions.Storage.Any() ? query.Where(item => item.ProductItemConfigurations.Any(
                config => config.PropertyValue.PropertyName.PropertyName1 == nameof(filterConditions.Storage) &&
                filterConditions.Storage.Contains(config.PropertyValue.PropertyValue1))) : query;
            //Filtering product items on the battery
            query = filterConditions.Battery != null && filterConditions.Battery.Any() ? query.Where(item => item.ProductItemConfigurations.Any(
                config => config.PropertyValue.PropertyName.PropertyName1 == nameof(filterConditions.Battery) &&
                filterConditions.Battery.Contains(config.PropertyValue.PropertyValue1))) : query;
            //Filtering product items on the screen size
            query = filterConditions.Screen_Size != null && filterConditions.Screen_Size.Any() ? query.Where(item => item.ProductItemConfigurations.Any(
                config => config.PropertyValue.PropertyName.PropertyName1 == nameof(filterConditions.Screen_Size) &&
                filterConditions.Screen_Size.Contains(config.PropertyValue.PropertyValue1))) : query;
            //Filtering product items on the resolution
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
            //Filtering product items on the processor
            query = filterConditions.Processor != null && filterConditions.Processor.Any() ? query.Where(item => item.ProductItemConfigurations.Any(
                config => config.PropertyValue.PropertyName.PropertyName1 == nameof(filterConditions.Processor) &&
                filterConditions.Processor.Contains(config.PropertyValue.PropertyValue1))) : query;

            query = filterConditions.MinPrice.HasValue ? query.Where(item => item.Price >= filterConditions.MinPrice.Value) : query;

            query = filterConditions.MaxPrice.HasValue ? query.Where(item => item.Price <= filterConditions.MaxPrice.Value) : query;
            query = query.Include(item => item.Product.ProductItemReviews)
                    .Include(item => item.SellerProductItems).ThenInclude(sellerProduct => sellerProduct.Seller)
                    .Include(item => item.ProductItemConfigurations).ThenInclude(config => config.PropertyValue).ThenInclude(propValue => propValue.PropertyName)
                    .Include(item => item.Product.Category).ThenInclude(category => category.PropertyNames);
            if (sortConditions.SortOnPrice)
            {
                query = sortConditions.SortByPriceAsc ? query.OrderBy(item => item.Price) : query.OrderByDescending(item => item.Price);
            }
            if (sortConditions.SortOnRating)
            {
                query = sortConditions.SortByRatingAsc ? query.OrderBy(item => item.Product.ProductItemReviews.Average(rating => rating.Rating))
                    : query.OrderByDescending(item => item.Product.ProductItemReviews.Average(rating => rating.Rating));
            }
            var productItems = await query.ToListAsync();
            var totalResultCount = productItems.Count;
            var filteredResults = productItems.Skip((filterConditions.Page - 1) * pageSize).Take(pageSize).ToList();
            var ListOfProductItemCardDto = mapper.Map<List<ProductItemCardDto>>(filteredResults);
            var finalFilteredResults = new PaginatedFilterResults
            {
                TotalFilterResults = totalResultCount,
                FilteredProductItems = ListOfProductItemCardDto
            };
            return finalFilteredResults;
        }

        public async Task<ProductItemDetailedPageDto?> GetDetailedProductItem(Guid productItemId)
        {
            var productItem = await dbContext.ProductItemDetails.Include(item => item.Product)
                    .Include(item => item.SellerProductItems).ThenInclude(sellerProduct => sellerProduct.Seller)
                    .Include(item => item.Product.ProductItemReviews)
                    .Include(item => item.ProductItemConfigurations).ThenInclude(config => config.PropertyValue).ThenInclude(propValue => propValue.PropertyName)
                    .Include(item => item.Product.Category).ThenInclude(category => category.PropertyNames).
                    FirstOrDefaultAsync(item => item.ProductItemId == productItemId);
            var productDescriptionWithVariants = productItem != null ? new ProductItemDetailedPageDto
            {
                ProductItemDetails = mapper.Map<ProductItemCardDto>(productItem),
                AvailableVariantOptions = productItem.Product.Category.PropertyNames
                .Select(property => new PropertiesOfVariants
                {
                    Name = property.PropertyName1,
                    Values =
                dbContext.ProductItemConfigurations.Include(config => config.ProductItem).Where(config => config.PropertyValue.PropertyName.PropertyName1 == property.PropertyName1 && config.ProductItem.ProductId == productItem.ProductId)
                .Select(values => values.PropertyValue.PropertyValue1).Distinct().ToList()
                }).ToDictionary(key => key.Name, value => value.Values)
            } : null;
            return productDescriptionWithVariants;
        }

        /*To this Method we send a single filterCondition i.e. either Colour or RAM or Storage (a filter property of product)
         and what are the values of the properties that we need in return 
         We get a reply with the productItemDetailCard which is found first of the chosen filterCondition 
         and available specifications as a list with key value pair*/
        public async Task<ProductVariantDetailedPageDto> FilterProductVariant(Guid productId, FilterVariantParametersDto filterConditions, List<string> featuresDataNeeded)
        {
            var query = dbContext.ProductItemDetails.Where(item => item.ProductId == productId);
            query = query.Include(item => item.Product)
                .Include(item => item.Product.ProductItemReviews)
                .Include(item => item.SellerProductItems).ThenInclude(sellerProduct => sellerProduct.Seller)
                .Include(item => item.ProductItemConfigurations).ThenInclude(config => config.PropertyValue).ThenInclude(value => value.PropertyName)
                .Include(item => item.Product.Category).ThenInclude(category => category.PropertyNames);
            query = !string.IsNullOrEmpty(filterConditions.Colour) ? query.Where(item => item.ProductItemConfigurations.Any(
                config => config.PropertyValue.PropertyName.PropertyName1 == nameof(filterConditions.Colour) &&
                config.PropertyValue.PropertyValue1 == filterConditions.Colour)) : query;
            query = !string.IsNullOrEmpty(filterConditions.Storage) ? query.Where(item => item.ProductItemConfigurations.Any(
                config => config.PropertyValue.PropertyName.PropertyName1 == nameof(filterConditions.Storage) &&
                config.PropertyValue.PropertyValue1 == filterConditions.Storage)) : query;
            query = !string.IsNullOrEmpty(filterConditions.RAM) ? query.Where(item => item.ProductItemConfigurations.Any(
                config => config.PropertyValue.PropertyName.PropertyName1 == nameof(filterConditions.RAM) &&
                config.PropertyValue.PropertyValue1 == filterConditions.RAM)) : query;
            var productVariants = await query.ToListAsync();
            var productVariantsDto = mapper.Map<List<ProductItemCardDto>>(productVariants);
            var AvailableFeatureAndOptions = new List<PropertiesOfVariants>();
            if (featuresDataNeeded.Any())
            {
                foreach (var feature in featuresDataNeeded)
                {
                    var availableFeatureOption = new PropertiesOfVariants
                    {
                        Name = feature,
                        Values = productVariantsDto.Select(variant => variant.Specifications[feature]).Distinct().ToList()
                    };
                    AvailableFeatureAndOptions.Add(availableFeatureOption);
                }
            }
            var finalVariantsAndFeatures = new ProductVariantDetailedPageDto
            {
                Variants = productVariantsDto,
                AvailableOptions = AvailableFeatureAndOptions.ToDictionary(key => key.Name, value => value.Values)
            };
            return finalVariantsAndFeatures;
        }
    }
}
