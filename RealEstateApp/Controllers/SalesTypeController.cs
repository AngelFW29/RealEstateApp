using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Catalog;

namespace RealEstateApp.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class SaleTypeController : Controller
    {
        private readonly ISaleTypeService _salesTypeService;

        public SaleTypeController(ISaleTypeService salesTypeService)
        {
            _salesTypeService = salesTypeService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var list = await _salesTypeService.GetAllAsync(cancellationToken);
            return View(list); 
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("Create", new SaveCatalogViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SaveCatalogViewModel vm, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return View("Create", vm);

            var result = await _salesTypeService.CreateAsync(vm, cancellationToken);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Error al crear el registro.");
                return View("Create", vm);
            }

            TempData["Success"] = "Registro creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var vm = await _salesTypeService.GetByIdAsync(id, cancellationToken);
            if (vm == null) return NotFound();

            return View("create", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SaveCatalogViewModel vm, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return View("create", vm);

            var result = await _salesTypeService.UpdateAsync(vm.Id, vm, cancellationToken);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Error al actualizar el registro.");
                return View("create", vm);
            }

            TempData["Success"] = "Registro actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var vm = await _salesTypeService.GetByIdAsync(id, cancellationToken);
            if (vm == null) return NotFound();

            return View(vm);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
        {
            await _salesTypeService.DeleteAsync(id, cancellationToken);
            TempData["Success"] = "Registro eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
