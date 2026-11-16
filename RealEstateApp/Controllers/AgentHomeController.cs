using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RealEstateApp.Core.Application.Dtos.Chat;
using RealEstateApp.Core.Application.Dtos.Property;
using RealEstateApp.Core.Application.Exceptions;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Agent;
using RealEstateApp.Core.Application.ViewModels.Chat;
using RealEstateApp.Core.Application.ViewModels.Offer;
using RealEstateApp.Core.Application.ViewModels.Property;
using System.Security.Claims;

namespace RealEstateApp.Controllers
{
    [Authorize(Roles = "Agente")]
    public class AgentHomeController : Controller
    {
        private readonly IUserService _userService;
        private readonly IPropertyService _propertyService;
        private readonly IPropertyTypeService _propertyTypeService;
        private readonly IChatService _chatService;
        private readonly IOfferService _offerService;
        private readonly IMapper _mapper;

        public AgentHomeController(IUserService userService, IPropertyService propertyService, IPropertyTypeService propertyTypeService, IChatService chatService,
            IOfferService offerService, IMapper mapper)
        {
            _userService = userService;
            _propertyService = propertyService;
            _propertyTypeService = propertyTypeService;
            _chatService = chatService;
            _offerService = offerService;
            _mapper = mapper;
        }
        private string AgentId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        public async Task<IActionResult> Index(PropertyFilterViewModel filter)
        {
            var model = new HomeViewModel { Filter = filter };

            var propertyTypes = await _propertyTypeService.GetAllAsync();

            ViewData["PropertyTypes"] = propertyTypes
                .Select(pt => new SelectListItem
                {
                    Value = pt.Id.ToString(),
                    Text = pt.Name
                })
                .ToList();

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var properties = await _propertyService.GetByAgentAsync(AgentId, _mapper.Map<PropertyFilterDto>(filter));
                model.Properties = _mapper.Map<List<PropertyViewModel>>(properties);
            }
            catch (BusinessRuleException ex)
            {
                ModelState.AddModelError(nameof(filter.PropertyTypeId), ex.Message);
                return View(model);
            }

            if (model.Properties.Count == 0)
                model.EmptyMessage = filter.GetNoResultsMessage() ?? "No tiene propiedades registradas en este momento.";

            return View(model);
        }
        public async Task<IActionResult> Detail(int id)
        {
            var property = await _propertyService.GetAgentPropertyByIdAsync(AgentId, id);

            if (property is null)
            {
                TempData["ErrorMessage"] = "No tiene permisos para acceder a esta propiedad.";
                return RedirectToAction(nameof(Index));
            }

            var conversations = await _chatService.GetConversationsAsync(AgentId, id);
            var offerSummaries = await _offerService.GetClientSummariesAsync(AgentId, id);

            var model = new AgentPropertyDetailViewModel
            {
                Property = _mapper.Map<PropertyViewModel>(property),
                Conversations = _mapper.Map<List<ConversationSummaryViewModel>>(conversations),
                OfferSummaries = _mapper.Map<List<ClientOffersSummaryViewModel>>(offerSummaries)
            };

            return View(model);
        }
        public async Task<IActionResult> Conversation(int propertyId, string clientId)
        {
            var property = await _propertyService.GetAgentPropertyByIdAsync(AgentId, propertyId);

            if (property is null)
            {
                TempData["ErrorMessage"] = "No tiene permisos para acceder a esta propiedad.";
                return RedirectToAction(nameof(Index));
            }

            List<ChatMessageDto> messages;

            try
            {
                messages = await _chatService.GetAgentConversationAsync(AgentId, propertyId, clientId);
            }
            catch (BusinessRuleException ex)
            {
                return RedirectToDetail(propertyId, ex.Message);
            }

            var client = await _userService.GetBasicInfoByIdAsync(clientId);

            var model = new AgentConversationViewModel
            {
                PropertyId = propertyId,
                PropertyCode = property.Code,
                ClientName = client?.FullName ?? string.Empty,
                Messages = _mapper.Map<List<ChatMessageViewModel>>(messages),
                Reply = new SendReplyViewModel { PropertyId = propertyId, ClientId = clientId }
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendReply(SendReplyViewModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToConversation(model.PropertyId, model.ClientId, FirstError());

            try
            {
                await _chatService.SendFromAgentAsync(AgentId, model.PropertyId, model.ClientId, model.Content);
            }
            catch (BusinessRuleException ex)
            {
                return RedirectToConversation(model.PropertyId, model.ClientId, ex.Message);
            }

            return RedirectToConversation(model.PropertyId, model.ClientId);
        }
        public async Task<IActionResult> ClientOffers(int propertyId, string clientId)
        {
            var property = await _propertyService.GetAgentPropertyByIdAsync(AgentId, propertyId);

            if (property is null)
            {
                TempData["ErrorMessage"] = "No tiene permisos para acceder a esta propiedad.";
                return RedirectToAction(nameof(Index));
            }

            var offers = await _offerService.GetOffersByClientAsync(AgentId, propertyId, clientId);

            if (offers.Count == 0)
                return RedirectToDetail(propertyId, "El cliente seleccionado no tiene ofertas para esta propiedad.");

            var client = await _userService.GetBasicInfoByIdAsync(clientId);

            var model = new AgentClientOffersViewModel
            {
                PropertyId = propertyId,
                PropertyCode = property.Code,
                ClientId = clientId,
                ClientName = client?.FullName ?? string.Empty,
                Offers = _mapper.Map<List<OfferViewModel>>(offers)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptOffer(int propertyId, int offerId, string clientId)
        {
            try
            {
                await _offerService.AcceptOfferAsync(AgentId, propertyId, offerId);
                TempData["SuccessMessage"] = "La oferta fue aceptada correctamente y la propiedad fue marcada como vendida.";
            }
            catch (BusinessRuleException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(ClientOffers), new { propertyId, clientId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectOffer(int propertyId, int offerId, string clientId)
        {
            try
            {
                await _offerService.RejectOfferAsync(AgentId, propertyId, offerId);
                TempData["SuccessMessage"] = "La oferta fue rechazada correctamente.";
            }
            catch (BusinessRuleException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(ClientOffers), new { propertyId, clientId });
        }

        #region Private methods
        private IActionResult RedirectToDetail(int propertyId, string message)
        {
            TempData["ErrorMessage"] = message;
            return RedirectToAction(nameof(Detail), new { id = propertyId });
        }
        private IActionResult RedirectToConversation(int propertyId, string clientId, string? errorMessage = null)
        {
            if (errorMessage is not null)
                TempData["ErrorMessage"] = errorMessage;

            return RedirectToAction(nameof(Conversation), new { propertyId, clientId });
        }
        private string? FirstError()
        {
            return ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .FirstOrDefault();
        }
        #endregion
    }
}