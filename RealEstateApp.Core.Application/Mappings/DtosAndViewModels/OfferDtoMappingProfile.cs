using AutoMapper;
using RealEstateApp.Core.Application.Dtos.Offer;
using RealEstateApp.Core.Application.ViewModels.Offer;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Core.Application.Mappings.EntitiesAndDtos
{
    public class OfferDtoMappingProfile : Profile
    {
        public OfferDtoMappingProfile()
        {
            CreateMap<OfferDto, OfferViewModel>();
        }
    }
}