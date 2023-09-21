using AutoMapper;
using ECommerce.Models.Domain;
using ECommerce.Models.DTOs;

namespace ECommerce.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AddCategoryRequestDto, Category>().ReverseMap();
            CreateMap<AddBrandRequestDto, Brand>().ReverseMap();
            CreateMap<BrandCategory,BrandCategoryDto>().ReverseMap();
            CreateMap<PropertyName, PropertyNameDto>()
                .ForMember(x => x.PropertyName, opt => opt.MapFrom(src => src.PropertyName1)).ReverseMap();
            CreateMap<Category,CategoryDto>()
                .ForMember(x => x.ChildCategories, opt => opt.MapFrom(src => src.InverseParentCategory)).ReverseMap();
            CreateMap<PropertyValue, PropertyValueDto>()
                .ForMember(x => x.PropertyValue, opt => opt.MapFrom(src => src.PropertyValue1)).ReverseMap();
            CreateMap<PropertyName, PropertyNameValueDto>()
                .ForMember(x => x.PropertyName, opt => opt.MapFrom(src => src.PropertyName1))
                .ForMember(x => x.PropertyValues, opt => opt.MapFrom(src => src.PropertyValues)).ReverseMap();
            CreateMap<Brand, BrandDto>().ReverseMap();
            CreateMap<ProductItemDetail, ProductItemCardDto>()
                .ForMember(x => x.CategoryId, opt => opt.MapFrom(src => src.Product.CategoryId))
                .ForMember(x => x.ProductItemDescription, opt => opt.MapFrom(src => src.Product.ProductDescription))
                .ForMember(x => x.Sellers, opt => opt.MapFrom(src => src.SellerProductItems.Select(sellerProducts =>new SellerDetailsDto {SellerId = sellerProducts.Seller.SellerId, SellerName = sellerProducts.Seller.SellerName }).ToList()))
                .ForMember(x => x.Rating, opt => opt.MapFrom(src => src.Product.ProductItemReviews.Count == 0 ? 0 : src.Product.ProductItemReviews.Average(product => product.Rating)))
                .ForMember(x => x.NumberOfRatings, opt => opt.MapFrom(src => src.Product.ProductItemReviews.Count))
                .ForMember(x => x.NumberOfReviews, opt => opt.MapFrom(src => src.Product.ProductItemReviews.Count(product => product.Review != null)))
                .ForMember(x => x.Specifications, opt => opt.MapFrom(src => src.Product.Category.PropertyNames
                .Select(property => new Properties { Name = property.PropertyName1, Value = src.ProductItemConfigurations.Where(config => config.PropertyValue.PropertyName.PropertyId == property.PropertyId)
                .Select(config => config.PropertyValue.PropertyValue1).FirstOrDefault()}).ToDictionary(key => key.Name, value => value.Value)))
                .ReverseMap();
            CreateMap<Address, AddressDto>().ReverseMap();
            CreateMap<AddAddressRequestDto, Address>().ReverseMap();
        }
    }
}
