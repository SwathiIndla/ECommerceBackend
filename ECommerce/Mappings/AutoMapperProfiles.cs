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
            CreateMap<PropertyName, PropertyNameDto>().
                ForMember(x => x.PropertyName, opt => opt.MapFrom(src => src.PropertyName1)).ReverseMap();
        }
    }
}
