using AutoMapper;
using RealEstateApp.Core.Application.Dtos.Favorite;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Core.Application.Mappings.EntitiesAndDtos
{
    public class FavoriteMappingProfile : Profile
    {
        public FavoriteMappingProfile()
        {
            CreateMap<Favorite, FavoriteDto>();

            CreateMap<FavoriteDto, Favorite>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.Property, o => o.Ignore());
        }
    }
}