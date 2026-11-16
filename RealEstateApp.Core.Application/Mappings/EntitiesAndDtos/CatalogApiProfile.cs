using AutoMapper;
using RealEstateApp.Core.Application.Dtos.Catalog;
using RealEstateApp.Core.Domain.Common;

namespace RealEstateApp.Core.Application.Mappings
{
    public class CatalogApiProfile : Profile
    {
        public CatalogApiProfile()
        {
            CreateMap<TypeEntity, CatalogItemDto>().ReverseMap();
            CreateMap<CatalogRequestDto, TypeEntity>();
        }
    }
}