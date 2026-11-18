using AutoMapper;
using RealEstateApp.Core.Application.Dtos.Catalog;
using RealEstateApp.Core.Domain.Common;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Core.Application.Mappings
{
    public class CatalogApiProfile : Profile
    {
        public CatalogApiProfile()
        {
            // Mapeos para DTOs de la API
            CreateMap<TypeEntity, CatalogItemDto>().ReverseMap();
            CreateMap<CatalogRequestDto, TypeEntity>();

            // Mapeos específicos por tipo concreto
            CreateMap<CatalogRequestDto, PropertyType>();
            CreateMap<CatalogRequestDto, SaleType>();
            CreateMap<CatalogRequestDto, Improvement>();
        }
    }
}