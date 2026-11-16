using Microsoft.AspNetCore.Mvc;

namespace RealEstateApi.Controllers
{
    public class SaleTypesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
