using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces;

namespace RealEstateApp.Controllers
{
    [Authorize(Roles = "Administrador")]
    [Route("[controller]")]
    public class AdminHomeController : Controller
    {
        private readonly IAgentManagementService _agentManagementService;

        public AdminHomeController(IAgentManagementService agentManagementService)
        {
            _agentManagementService = agentManagementService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var agents = await _agentManagementService.GetAllAsync(cancellationToken);
            return View(agents);
        }

        [HttpGet("change-status/{id}")]
        public async Task<IActionResult> ChangeStatus(string id, CancellationToken cancellationToken)
        {
            var vm = await _agentManagementService.GetByIdAsync(id, cancellationToken);
            if (vm == null)
            {
                TempData["Error"] = "El agente seleccionado no existe.";
                return RedirectToAction(nameof(Index));
            }

            return View(vm);
        }

        [HttpPost("change-status/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatusConfirmed(string id, CancellationToken cancellationToken)
        {
            var vm = await _agentManagementService.GetByIdAsync(id, cancellationToken);
            if (vm == null)
            {
                TempData["Error"] = "El agente seleccionado no existe.";
                return RedirectToAction(nameof(Index));
            }

            bool newStatus = !vm.IsActive;
            var result = await _agentManagementService.SetActiveAsync(id, newStatus, cancellationToken);

            if (!result.Success)
            {
                TempData["Error"] = result.FirstError;
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = newStatus ? "El agente fue activado correctamente." : "El agente fue inactivado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
        {
            var vm = await _agentManagementService.GetByIdAsync(id, cancellationToken);
            if (vm == null)
            {
                TempData["Error"] = "El agente seleccionado no existe.";
                return RedirectToAction(nameof(Index));
            }

            return View(vm);
        }

        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id, CancellationToken cancellationToken)
        {
            var result = await _agentManagementService.DeleteAsync(id, cancellationToken);
            if (!result.Success)
            {
                TempData["Error"] = result.FirstError;
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "El agente fue eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}