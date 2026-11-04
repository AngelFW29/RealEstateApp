using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RealEstateApp.Controllers
{
    [Authorize(Roles = "Agente")]
    public class AgentHomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}