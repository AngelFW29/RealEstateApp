using AutoMapper;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Services
{
    public class SaleTypeService : CatalogService<SaleType>, ISaleTypeService
    {
        public SaleTypeService(ICatalogRepository<SaleType> repository, IMapper mapper) : base(repository, mapper)
        {
        }

        protected override string SingularNameMale => "tipo de venta";

        protected override string SingularNameFemale => string.Empty;

        protected override bool IsFemale => false;
    }
}
