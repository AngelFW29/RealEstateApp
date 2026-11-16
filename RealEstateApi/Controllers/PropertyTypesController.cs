using RealEstateApp.Core.Application.Interfaces;

namespace RealEstateApi.Controllers
{
    public class PropertyTypesController : CatalogApiControllerBase
    {
        public PropertyTypesController(IPropertyTypeService propertyTypeService) : base(propertyTypeService)
        {
        }

        protected override string NotFoundMessage => "El tipo de propiedad solicitado no existe.";
    }
}