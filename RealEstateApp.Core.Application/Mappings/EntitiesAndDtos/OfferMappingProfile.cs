using AutoMapper;
using RealEstateApp.Core.Application.Dtos.Favorite;
using RealEstateApp.Core.Application.Dtos.Offer;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Core.Application.Mappings.EntitiesAndDtos
{
    public class OfferMappingProfile : Profile
    {
        public OfferMappingProfile()
        {
            CreateMap<Offer, OfferDto>();

            CreateMap<OfferDto, Offer>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.Status, o => o.Ignore())
                .ForMember(d => d.CreatedAt, o => o.Ignore())
                .ForMember(d => d.Property, o => o.Ignore());
        }
    }
}