﻿using AutoMapper;
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
            CreateMap<ProductItemDetail,RawProductItemCardDto>()
                .ForMember(x => x.CategoryId, opt => opt.MapFrom(src => src.Product.CategoryId))
                .ForMember(x => x.Rating, opt => opt.MapFrom(src => src.Product.ProductItemReviews.Average(product => product.Rating)))
                .ForMember(x => x.NumberOfRatings, opt => opt.MapFrom(src => src.Product.ProductItemReviews.Count()))
                .ForMember(x => x.NumberOfReviews, opt => opt.MapFrom(src => src.Product.ProductItemReviews.Count(product => product.Review != null)))
                .ForMember(x => x.Specifications, opt => opt.MapFrom(src => src.Product.Category.PropertyNames
                .Select(property => new Properties { Name = property.PropertyName1, Value = src.ProductItemConfigurations.Where(config => config.PropertyValue.PropertyName.PropertyId == property.PropertyId)
                .Select(config => config.PropertyValue.PropertyValue1).FirstOrDefault()}).ToList()))
                .ReverseMap();
            CreateMap<RawProductItemCardDto, ProductItemCardDto>()
                .ForMember(item => item.Specifications, opt => opt.MapFrom(src => src.Specifications.ToDictionary(key => key.Name, value => value.Value))).ReverseMap();
        }
    }
}