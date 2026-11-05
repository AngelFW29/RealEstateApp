using AutoMapper;
using RealEstateApp.Core.Application.Dtos.Property;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Core.Application.Mappings.EntitiesAndDtos
{
    public class PropertyMappingProfile : Profile
    {
        public PropertyMappingProfile()
        {
            CreateMap<Property, PropertyDto>()
                .ForMember(d => d.PropertyTypeName, o => o.MapFrom(s => s.PropertyType != null ? s.PropertyType.Name : string.Empty))
                .ForMember(d => d.SaleTypeName, o => o.MapFrom(s => s.SaleType != null ? s.SaleType.Name : string.Empty))
                .ForMember(d => d.MainImagePath, o => o.MapFrom(s => s.Images.Where(i => i.IsMain).Select(i => i.ImagePath).FirstOrDefault()))
                .ForMember(d => d.ImagePaths, o => o.MapFrom(s => s.Images.OrderBy(i => i.DisplayOrder).Select(i => i.ImagePath).ToList()))
                .ForMember(d => d.ImprovementNames, o => o.MapFrom(s => s.Improvements.Where(pi => pi.Improvement != null).Select(pi => pi.Improvement!.Name).ToList()))
                .ForMember(d => d.AgentName, o => o.Ignore())
                .ForMember(d => d.AgentPhone, o => o.Ignore())
                .ForMember(d => d.AgentEmail, o => o.Ignore())
                .ForMember(d => d.AgentPhotoPath, o => o.Ignore());

            CreateMap<PropertyDto, Property>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.Code, o => o.Ignore())
                .ForMember(d => d.AgentId, o => o.Ignore())
                .ForMember(d => d.Status, o => o.Ignore())
                .ForMember(d => d.CreatedAt, o => o.Ignore())
                .ForMember(d => d.PropertyType, o => o.Ignore())
                .ForMember(d => d.SaleType, o => o.Ignore())
                .ForMember(d => d.Images, o => o.Ignore())
                .ForMember(d => d.Improvements, o => o.Ignore())
                .ForMember(d => d.Favorites, o => o.Ignore())
                .ForMember(d => d.Messages, o => o.Ignore())
                .ForMember(d => d.Offers, o => o.Ignore());
        }
    }
}