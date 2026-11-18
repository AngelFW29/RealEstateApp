using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Services
{
    public class SaleTypeService : CatalogService<SaleType>, ISaleTypeService
    {
        public SaleTypeService(ISaleTypeRepository saleTypeRepository, IMapper mapper)
            : base(saleTypeRepository, mapper) { }

        protected override string SingularNameMale => "tipo de venta";
        protected override string SingularNameFemale => string.Empty;
        protected override bool IsFemale => false;
    }
}
