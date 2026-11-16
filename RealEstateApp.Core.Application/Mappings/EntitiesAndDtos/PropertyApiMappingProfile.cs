using AutoMapper;
using RealEstateApp.Core.Application.Dtos.Property;
using RealEstateApp.Core.Domain.Common.Enums;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Core.Application.Mappings.EntitiesAndDtos
{
    public class PropertyApiMappingProfile : Profile
    {
        public PropertyApiMappingProfile()
        {
            CreateMap<Property, PropertyApiDto>()
                .ForMember(d => d.PropertyType, o => o.MapFrom(s => s.PropertyType != null ? s.PropertyType.Name : string.Empty))
                .ForMember(d => d.SaleType, o => o.MapFrom(s => s.SaleType != null ? s.SaleType.Name : string.Empty))
                .ForMember(d => d.LandSize, o => o.MapFrom(s => s.SizeInMeters))
                .ForMember(d => d.Improvements, o => o.MapFrom(s => s.Improvements
                    .Where(pi => pi.Improvement != null)
                    .Select(pi => pi.Improvement!.Name)
                    .ToList()))
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status == PropertyStatus.Available ? "Disponible" : "Vendida"))
                .ForMember(d => d.AgentName, o => o.MapFrom(_ => string.Empty));
        }
    }
}