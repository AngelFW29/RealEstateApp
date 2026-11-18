using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Catalog;

namespace RealEstateApp.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ImprovementController : Controller
    {
        private readonly IPropertyImprovementService _improvementService;

        public ImprovementController(IPropertyImprovementService improvementService)
        {
            _improvementService = improvementService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var list = await _improvementService.GetAllAsync(cancellationToken);
            return View(list);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("create", new SaveCatalogViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SaveCatalogViewModel vm, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return View("create", vm);

            var result = await _improvementService.CreateAsync(vm, cancellationToken);
            if (!result.Success)
            {
                TempData["Error"] = result.FirstError;
                return View("create", vm);
            }

            TempData["Success"] = "Registro creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var vm = await _improvementService.GetByIdAsync(id, cancellationToken);
            if (vm == null) return NotFound();

            return View("create", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SaveCatalogViewModel vm, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return View("create", vm);

            var result = await _improvementService.UpdateAsync(vm.Id, vm, cancellationToken);
            if (!result.Success)
            {
                TempData["Error"] = result.FirstError;
                return View("create", vm);
            }

            TempData["Success"] = "Registro actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var vm = await _improvementService.GetByIdAsync(id, cancellationToken);
            if (vm == null) return NotFound();

            return View(vm);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
        {
            await _improvementService.DeleteAsync(id, cancellationToken);
            TempData["Success"] = "Registro eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}