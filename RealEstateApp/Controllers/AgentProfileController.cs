using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application;
using RealEstateApp.Core.Application.Dtos.Property;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Agent;
using RealEstateApp.Infrastructure.Identity.Entities;
using System.Security.Claims;

namespace RealEstateApp.Controllers
{
    [Authorize(Roles = "Agente")]
    public class AgentProfileController : Controller
    {
        private readonly IAgentProfileService _profileService;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public AgentProfileController(IAgentProfileService profileService, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _profileService = profileService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Challenge();

            var result = await _profileService.GetAsync(userId, cancellationToken);
            if (!result.Success)
            {
                TempData["Error"] = result.Errors;
                return View(new AgentProfileViewModel());
            }

            return View(result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(AgentProfileViewModel vm, IFormFile? profileImage, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Challenge();

            if (profileImage != null && profileImage.Length > 0)
            {
                if (!ImageHelper.IsValid(profileImage))
                {
                    ModelState.AddModelError("profileImage", "El formato de la imagen no es válido (solo se permiten JPG y PNG).");
                }
            }

            if (!ModelState.IsValid)
            {
                var currentData = await _profileService.GetAsync(userId, cancellationToken);
                if (currentData.Success)
                {
                    vm.CurrentPhotoUrl = currentData.Data.CurrentPhotoUrl;
                }
                return View(vm);
            }

            IFormFile? imageDto = null;
            var updateResult = await _profileService.UpdateAsync(userId, vm, imageDto, cancellationToken);
            if (!updateResult.Success)
            {
                var errorMessage = updateResult.FirstError ?? updateResult.Message ?? "Error al actualizar el perfil.";
                ModelState.AddModelError(string.Empty, errorMessage);

                var currentData = await _profileService.GetAsync(userId, cancellationToken);
                if (currentData.Success)
                {
                    vm.CurrentPhotoUrl = currentData.Data.CurrentPhotoUrl;
                }
                return View(vm);
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _signInManager.RefreshSignInAsync(user);
            }

            TempData["Success"] = "Su perfil fue actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
