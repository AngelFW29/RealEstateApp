using RealEstateApp.Core.Application.Interfaces;

namespace RealEstateApi.Controllers
{
    public class SaleTypesController : CatalogApiControllerBase
    {
        public SaleTypesController(ISaleTypeService saleTypeService) : base(saleTypeService)
        {
        }

        protected override string NotFoundMessage => "El tipo de venta solicitado no existe.";
    }
}