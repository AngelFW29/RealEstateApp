using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Dtos.Property;
using RealEstateApp.Core.Application.Exceptions;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Property;
using System.Security.Claims;

namespace RealEstateApp.Controllers
{
    [Authorize(Roles = "Cliente")]
    public class ClientHomeController : Controller
    {
        private readonly IPropertyService _propertyService;
        private readonly IFavoriteService _favoriteService;
        private readonly IMapper _mapper;
        public ClientHomeController(IPropertyService propertyService, IFavoriteService favoriteService, IMapper mapper)
        {
            _propertyService = propertyService;
            _favoriteService = favoriteService;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index(PropertyFilterViewModel filter)
        {
            var model = await BuildModelAsync(filter, _propertyService.GetAvailableAsync, emptyDefault: null);
            return View(model);
        }
        public async Task<IActionResult> MyProperties(PropertyFilterViewModel filter)
        {
            var model = await BuildModelAsync(filter,
                f => _favoriteService.GetFavoritePropertiesAsync(ClientId, f),
                emptyDefault: "No tiene propiedades favoritas disponibles en este momento.");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleFavorite(int propertyId, string? returnUrl)
        {
            try
            {
                var added = await _favoriteService.ToggleAsync(ClientId, propertyId);

                TempData["SuccessMessage"] = added
                    ? "La propiedad fue agregada a sus favoritas correctamente."
                    : "La propiedad fue eliminada de sus favoritas correctamente.";
            }
            catch (BusinessRuleException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return Url.IsLocalUrl(returnUrl)
                ? LocalRedirect(returnUrl!)
                : RedirectToAction(nameof(Index));
        }

        #region Private methods
        private string ClientId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        private async Task<HomeViewModel> BuildModelAsync(
            PropertyFilterViewModel filter, Func<PropertyFilterDto,
            Task<List<PropertyDto>>> loadProperties, string? emptyDefault)
        {
            var model = new HomeViewModel { Filter = filter };

            if (!ModelState.IsValid)
                return model;

            try
            {
                var properties = await loadProperties(_mapper.Map<PropertyFilterDto>(filter));
                model.Properties = _mapper.Map<List<PropertyViewModel>>(properties);
            }
            catch (BusinessRuleException ex)
            {
                ModelState.AddModelError(nameof(filter.PropertyTypeId), ex.Message);
                return model;
            }

            await MarkFavoritesAsync(model.Properties);

            if (model.Properties.Count == 0)
                model.EmptyMessage = filter.GetNoResultsMessage() ?? emptyDefault;

            return model;
        }
        private async Task MarkFavoritesAsync(List<PropertyViewModel> properties)
        {
            var favoriteIds = (await _favoriteService.GetFavoritePropertyIdsAsync(ClientId)).ToHashSet();

            foreach (var property in properties)
                property.IsFavorite = favoriteIds.Contains(property.Id);
        }
        #endregion
    }
}