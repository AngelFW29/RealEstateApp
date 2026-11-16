using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Dtos.Property;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Property;
using System.Security.Claims;

namespace RealEstateApp.Controllers
{
    [Authorize(Roles = "Agente")]
    public class PropertyMaintenanceController : Controller
    {
        private readonly IPropertyService _propertyService;

        public PropertyMaintenanceController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var agentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(agentId)) return Challenge();

            var properties = await _propertyService.GetAllByAgentAsync(agentId);
            return View(properties);
        }

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            var agentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(agentId)) return Challenge();

            var checkResult = await _propertyService.CheckCatalogsAsync();
            if (!checkResult.Success)
            {
                TempData["Error"] = checkResult.FirstError;
                return RedirectToAction(nameof(Index));
            }

            var vm = new SavePropertyViewModel();
            await _propertyService.PopulateFormAsync(vm, agentId);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SavePropertyViewModel vm, List<IFormFile> images, CancellationToken cancellationToken)
        {
            var agentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!ModelState.IsValid)
            {
                await _propertyService.PopulateFormAsync(vm, agentId);
                return View(vm);
            }

            var imageDtos = new List<ImageUploadDto>();
            foreach (var img in images)
            {
                if (img.Length > 0)
                {
                    var ms = new MemoryStream();
                    await img.CopyToAsync(ms, cancellationToken);
                    ms.Position = 0;
                    imageDtos.Add(new ImageUploadDto
                    {
                        FileName = img.FileName,
                        ContentType = img.ContentType,
                        Content = ms
                    });
                }
            }

            var result = await _propertyService.CreateAsync(vm, imageDtos, agentId);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Error al crear la propiedad.");
                await _propertyService.PopulateFormAsync(vm, agentId);
                return View(vm);
            }

            TempData["Success"] = "La propiedad fue creada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var agentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(agentId)) return Challenge();

            var result = await _propertyService.GetForEditAsync(id, agentId, cancellationToken);
            if (!result.Success)
            {
                TempData["Error"] = result.FirstError;
                if (result.NotFound) return RedirectToAction(nameof(Index));
                return RedirectToAction(nameof(Index));
            }

            return View(result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SavePropertyViewModel vm, List<IFormFile> newImages, CancellationToken cancellationToken)
        {
            var agentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(agentId)) return Challenge();

            var imageDtos = new List<ImageUploadDto>();
            foreach (var img in newImages)
            {
                if (img.Length > 0)
                {
                    var ms = new MemoryStream();
                    await img.CopyToAsync(ms, cancellationToken);
                    ms.Position = 0;
                    imageDtos.Add(new ImageUploadDto
                    {
                        FileName = img.FileName,
                        ContentType = img.ContentType,
                        Content = ms
                    });
                }
            }

            var result = await _propertyService.UpdateAsync(vm, imageDtos, agentId);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.FirstError ?? "Error al actualizar la propiedad.");
                await _propertyService.PopulateFormAsync(vm, agentId);
                return View(vm);
            }

            TempData["Success"] = "La propiedad fue actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var agentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(agentId)) return Challenge();

            var result = await _propertyService.GetForDeleteAsync(id, agentId);
            if (!result.Success)
            {
                TempData["Error"] = result.FirstError;
                return RedirectToAction(nameof(Index));
            }

            return View(result.Data);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
        {
            var agentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(agentId)) return Challenge();

            var result = await _propertyService.DeleteAsync(id, agentId);
            if (!result.Success)
            {
                TempData["Error"] = result.FirstError;
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "La propiedad fue eliminada correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
