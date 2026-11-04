using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RealEstateApp.Controllers
{
    [Authorize(Roles = "Cliente")]
    public class ClientHomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}