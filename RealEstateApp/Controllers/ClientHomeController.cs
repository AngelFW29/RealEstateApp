using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Dtos.Property;
using RealEstateApp.Core.Application.Exceptions;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Chat;
using RealEstateApp.Core.Application.ViewModels.Offer;
using RealEstateApp.Core.Application.ViewModels.Property;
using System.Security.Claims;

namespace RealEstateApp.Controllers
{
    [Authorize(Roles = "Cliente")]
    public class ClientHomeController : Controller
    {
        private readonly IPropertyService _propertyService;
        private readonly IFavoriteService _favoriteService;
        private readonly IChatService _chatService;
        private readonly IOfferService _offerService;
        private readonly IMapper _mapper;
        public ClientHomeController(IPropertyService propertyService, IFavoriteService favoriteService,
            IChatService chatService, IOfferService offerService, IMapper mapper)
        {
            _propertyService = propertyService;
            _favoriteService = favoriteService;
            _chatService = chatService;
            _offerService = offerService;
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
        public async Task<IActionResult> Detail(int id)
        {
            var property = await _propertyService.GetAvailableByIdAsync(id);

            if (property is null)
            {
                TempData["ErrorMessage"] = "La propiedad solicitada no existe o no se encuentra disponible.";
                return RedirectToAction(nameof(Index));
            }

            var messages = await _chatService.GetClientConversationAsync(ClientId, id);
            var offers = await _offerService.GetClientOffersAsync(ClientId, id);

            var model = new ClientPropertyDetailViewModel
            {
                Property = _mapper.Map<PropertyViewModel>(property),
                Messages = _mapper.Map<List<ChatMessageViewModel>>(messages),
                Offers = _mapper.Map<List<OfferViewModel>>(offers),
                OfferBlockReason = await _offerService.GetSendBlockReasonAsync(ClientId, id),
                NewMessage = new SendMessageViewModel { PropertyId = id },
                NewOffer = new SendOfferViewModel { PropertyId = id }
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(SendMessageViewModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToDetail(model.PropertyId, FirstError());

            try
            {
                await _chatService.SendFromClientAsync(ClientId, model.PropertyId, model.Content);
            }
            catch (BusinessRuleException ex)
            {
                return RedirectToDetail(model.PropertyId, ex.Message);
            }

            return RedirectToDetail(model.PropertyId);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendOffer(SendOfferViewModel model)
        {
            var attempted = ModelState[nameof(model.Amount)]?.AttemptedValue;

            if (model.Amount is null && !string.IsNullOrWhiteSpace(attempted))
                return RedirectToDetail(model.PropertyId, "El monto de la oferta debe ser un valor numérico mayor que cero.");

            if (!ModelState.IsValid)
                return RedirectToDetail(model.PropertyId, FirstError());

            try
            {
                await _offerService.SendOfferAsync(ClientId, model.PropertyId, model.Amount!.Value);
            }
            catch (BusinessRuleException ex)
            {
                return RedirectToDetail(model.PropertyId, ex.Message);
            }

            return RedirectToDetail(model.PropertyId);
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
        private IActionResult RedirectToDetail(int propertyId, string? errorMessage = null)
        {
            if (errorMessage is not null)
                TempData["ErrorMessage"] = errorMessage;

            return RedirectToAction(nameof(Detail), new { id = propertyId });
        }
        private string? FirstError()
        {
            return ModelState.Values
                .SelectMany(v => v.Errors).Select(e => e.ErrorMessage).FirstOrDefault();
        }
        #endregion
    }
}