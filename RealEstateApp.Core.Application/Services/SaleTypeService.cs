using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Services
{
    public class SaleTypeService : CatalogService<SaleType>, ISaleTypeService
    {
        private readonly ISaleTypeRepository _saleTypeRepository;
        public SaleTypeService(ICatalogRepository<SaleType> repository, ISaleTypeRepository saleTypeRepository, IMapper mapper) : base(repository, mapper)
        {
            _saleTypeRepository = saleTypeRepository;
        }

        protected override string SingularNameMale => "tipo de venta";

        protected override string SingularNameFemale => string.Empty;

        protected override bool IsFemale => false;
    }
}
