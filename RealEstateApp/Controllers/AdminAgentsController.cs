using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Staff;
using System.Security.Claims;

namespace RealEstateApp.Controllers
{
    [Authorize(Roles = "Administrador")]
    [Route("admin/agents")]
    public class AdminAgentsController : Controller
    {
        private readonly IAgentService _agentService;
        private readonly IAgentManagementService _agentManagementService;

        public AdminAgentsController(IAgentService agentService, IAgentManagementService agentManagementService)
        {
            _agentService = agentService;
            _agentManagementService = agentManagementService;
        }

        private string GetCurrentUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        [HttpGet("")]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var model = await _agentService.GetAllAsync(GetCurrentUserId() ,cancellationToken);
            return View(model);
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            return View(new CreateStaffUserViewModel());
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateStaffUserViewModel vm, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return View(vm);

            var result = await _agentService.CreateAsync(vm, cancellationToken);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.FirstError);
                return View(vm);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(string id, CancellationToken cancellationToken)
        {
            var vm = await _agentService.GetForEditAsync(id, GetCurrentUserId(), cancellationToken);
            if (vm == null)
            {
                TempData["Error"] = "El agente seleccionado no existe.";
                return RedirectToAction(nameof(Index));
            }

            return View(vm);
        }

        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EditStaffUserViewModel vm, CancellationToken cancellationToken)
        {
            if (id != vm.Id) return BadRequest();
            if (!ModelState.IsValid) return View(vm);

            var result = await _agentService.UpdateAsync(vm, GetCurrentUserId(), cancellationToken);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.FirstError);
                return View(vm);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("change-status/{id}")]
        public async Task<IActionResult> ChangeStatus(string id, CancellationToken cancellationToken)
        {
            var vm = await _agentService.GetForEditAsync(id, GetCurrentUserId(), cancellationToken);
            if (vm == null)
            {
                TempData["Error"] = "El agente seleccionado no existe.";
                return RedirectToAction(nameof(Index));
            }
            return View(vm);
        }

        [HttpPost("change-status/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatusConfirmed(string id, bool isActive, CancellationToken cancellationToken)
        {
            var result = await _agentService.SetActiveAsync(id, isActive, GetCurrentUserId(), cancellationToken);
            if (!result.Success)
            {
                TempData["Error"] = result.FirstError;
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = result.Message;
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
