using RealEstateApp.Core.Application.Interfaces;

namespace RealEstateApi.Controllers
{
    public class ImprovementsController : CatalogApiControllerBase
    {
        public ImprovementsController(IPropertyImprovementService improvementService) : base(improvementService)
        {
        }

        protected override string NotFoundMessage => "La mejora solicitada no existe.";
    }
}