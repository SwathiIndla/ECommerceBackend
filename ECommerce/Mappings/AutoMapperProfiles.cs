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
        }
    }
}
