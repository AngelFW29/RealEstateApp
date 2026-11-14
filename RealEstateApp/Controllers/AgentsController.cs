using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RealEstateApp.Core.Application.Dtos.Property;
using RealEstateApp.Core.Application.Exceptions;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.Services;
using RealEstateApp.Core.Application.ViewModels.Agent;
using RealEstateApp.Core.Application.ViewModels.Property;

namespace RealEstateApp.Controllers
{
    public class AgentsController : Controller
    {
        private readonly IUserService _userService;
        private readonly IPropertyService _propertyService;
        private readonly IPropertyTypeService _propertyTypeService;
        private readonly IMapper _mapper;
        public AgentsController(IUserService userService, IPropertyService propertyService, IPropertyTypeService propertyTypeService, IMapper mapper)
        {
            _userService = userService;
            _propertyService = propertyService;
            _propertyTypeService = propertyTypeService;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index(string? searchTerm)
        {
            var agents = await _userService.GetActiveAgentsAsync(searchTerm);

            var model = new AgentListViewModel
            {
                SearchTerm = searchTerm,
                Agents = _mapper.Map<List<AgentViewModel>>(agents)
            };

            if (model.Agents.Count == 0 && !string.IsNullOrWhiteSpace(searchTerm))
                model.EmptyMessage = "No se encontraron agentes activos con el nombre ingresado.";

            return View(model);
        }
        public async Task<IActionResult> Properties(string id, PropertyFilterViewModel filter)
        {
            var agent = (await _userService.GetActiveAgentsAsync()).FirstOrDefault(a => a.Id == id);

            var propertyTypes = await _propertyTypeService.GetAllAsync();

            var propertyTypeItems = propertyTypes
                .Select(pt => new SelectListItem
                {
                    Value = pt.Id.ToString(),
                    Text = pt.Name
                })
                .ToList();
            ViewData["PropertyTypes"] = propertyTypeItems;

            if (agent is null)
            {
                TempData["ErrorMessage"] = "El agente solicitado no existe o no se encuentra disponible.";
                return RedirectToAction(nameof(Index));
            }

            var model = new AgentPropertiesViewModel
            {
                Agent = _mapper.Map<AgentViewModel>(agent),
                Filter = filter
            };

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var filterDto = _mapper.Map<PropertyFilterDto>(filter);
                filterDto.AgentId = id;

                var properties = await _propertyService.GetAvailableAsync(filterDto);
                model.Properties = _mapper.Map<List<PropertyViewModel>>(properties);
            }
            catch (BusinessRuleException ex)
            {
                ModelState.AddModelError(nameof(filter.PropertyTypeId), ex.Message);
                return View(model);
            }

            if (model.Properties.Count == 0)
                model.EmptyMessage = filter.GetNoResultsMessage()
                    ?? "Este agente no tiene propiedades disponibles en este momento.";

            return View(model);
        }
    }
}