using AutoMapper;
using RealEstateApp.Core.Application.Dtos.Property;
using RealEstateApp.Core.Application.ViewModels.Property;

namespace RealEstateApp.Core.Application.Mappings.DtosAndViewModels
{
    public class PropertyDtoMappingProfile : Profile
    {
        public PropertyDtoMappingProfile()
        {
            CreateMap<PropertyDto, PropertyViewModel>()
                .ForMember(d => d.IsFavorite, o => o.Ignore());

            CreateMap<PropertyFilterViewModel, PropertyFilterDto>()
                .ForMember(d => d.AgentId, o => o.Ignore());
        }
    }
}