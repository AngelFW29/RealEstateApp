using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Staff;
using System.Security.Claims;

namespace RealEstateApp.Controllers
{
    [Authorize(Roles = "Administrador")]
    [Route("admin/administrators")]
    public class AdministratorController : Controller
    {
        private readonly IAdministratorService _adminService;

        public AdministratorController(IAdministratorService adminService)
        {
            _adminService = adminService;
        }

        private string GetCurrentUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        [HttpGet("")]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var model = await _adminService.GetAllAsync(GetCurrentUserId(), cancellationToken);
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

            var result = await _adminService.CreateAsync(vm, cancellationToken);
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
            var vm = await _adminService.GetForEditAsync(id, GetCurrentUserId(), cancellationToken);
            if (vm == null)
            {
                TempData["Error"] = "El administrador seleccionado no existe.";
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

            var result = await _adminService.UpdateAsync(vm, GetCurrentUserId(), cancellationToken);
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
            var vm = await _adminService.GetForEditAsync(id, GetCurrentUserId(), cancellationToken);
            if (vm == null)
            {
                TempData["Error"] = "El administrador seleccionado no existe.";
                return RedirectToAction(nameof(Index));
            }
            return View(vm);
        }

        [HttpPost("change-status/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatusConfirmed(string id, bool isActive, CancellationToken cancellationToken)
        {
            var result = await _adminService.SetActiveAsync(id, isActive, GetCurrentUserId(), cancellationToken);
            if (!result.Success)
            {
                TempData["Error"] = result.FirstError;
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}
