using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Catalog;

namespace RealEstateApp.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class PropertyTypeController : Controller
    {
        private readonly IPropertyTypeService _propertyTypeService;

        public PropertyTypeController(IPropertyTypeService propertyTypeService)
        {
            _propertyTypeService = propertyTypeService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var list = await _propertyTypeService.GetAllAsync(cancellationToken);
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

            var result = await _propertyTypeService.CreateAsync(vm, cancellationToken);
            if (!result.Success)
            {
                TempData["Error"] = result.FirstError;
                return View("Create", vm);
            }

            TempData["Success"] = "Registro creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var vm = await _propertyTypeService.GetByIdAsync(id, cancellationToken);
            if (vm == null) return NotFound();

            return View("Create", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SaveCatalogViewModel vm, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return View("Create", vm);

            var result = await _propertyTypeService.UpdateAsync(vm.Id, vm, cancellationToken);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Error al actualizar el registro.");
                return View("Create", vm);
            }

            TempData["Success"] = "Registro actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var vm = await _propertyTypeService.GetByIdAsync(id, cancellationToken);
            if (vm == null) return NotFound();

            return View(vm);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
        {
            await _propertyTypeService.DeleteAsync(id, cancellationToken);
            TempData["Success"] = "Registro eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
