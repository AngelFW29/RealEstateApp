using AutoMapper;
using RealEstateApp.Core.Application.ViewModels.Catalog;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Core.Application.Mappings.DtosAndViewModels
{
    public class CatalogMappingProfile : Profile
    {
        public CatalogMappingProfile()
        {
            CreateMap<PropertyType, CatalogListViewModel>().ReverseMap();
            CreateMap<PropertyType, SaveCatalogViewModel>().ReverseMap();

            CreateMap<SaleType, CatalogListViewModel>().ReverseMap();
            CreateMap<SaleType, SaveCatalogViewModel>().ReverseMap();

            CreateMap<Improvement, CatalogListViewModel>().ReverseMap();
            CreateMap<Improvement, SaveCatalogViewModel>().ReverseMap();
        }
    }
}
