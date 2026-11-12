using AutoMapper;
using RealEstateApp.Core.Application.Dtos.Offer;
using RealEstateApp.Core.Application.ViewModels.Chat;
using RealEstateApp.Core.Application.ViewModels.Offer;

namespace RealEstateApp.Core.Application.Mappings.EntitiesAndDtos
{
    public class OfferDtoMappingProfile : Profile
    {
        public OfferDtoMappingProfile()
        {
            CreateMap<OfferDto, OfferViewModel>();
            CreateMap<ClientOffersSummaryDto, ClientOffersSummaryViewModel>();
        }
    }
}