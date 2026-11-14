using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RealEstateApp.Core.Application.Dtos.Property;
using RealEstateApp.Core.Application.Exceptions;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Property;

namespace RealEstateApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPropertyService _propertyService;
        private readonly IPropertyTypeService _propertyTypeService;
        private readonly IMapper _mapper;
        public HomeController(IPropertyService propertyService, IPropertyTypeService propertyTypeService, IMapper mapper)
        {
            _propertyService = propertyService;
            _propertyTypeService = propertyTypeService;
            _mapper = mapper;
        }
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

            if (!ModelState.IsValid) return View(model);

            try
            {
                var properties = await _propertyService.GetAvailableAsync(_mapper.Map<PropertyFilterDto>(filter));
                model.Properties = _mapper.Map<List<PropertyViewModel>>(properties);
            }
            catch (BusinessRuleException ex)
            {
                ModelState.AddModelError(nameof(filter.PropertyTypeId), ex.Message);
                return View(model);
            }

            model.EmptyMessage = model.Properties.Count == 0 ? filter.GetNoResultsMessage() : null;
            return View(model);
        }
        public async Task<IActionResult> Detail(int id)
        {
            var property = await _propertyService.GetAvailableByIdAsync(id);

            if (property is null)
            {
                TempData["ErrorMessage"] = "La propiedad solicitada no existe o no se encuentra disponible.";
                return RedirectToAction(nameof(Index));
            }

            return View(_mapper.Map<PropertyViewModel>(property));
        }

        [Authorize]
        public IActionResult RedirectByRole()
        {
            if (User.IsInRole("Agente"))
                return RedirectToAction("Index", "AgentHome");

            if (User.IsInRole("Cliente"))
                return RedirectToAction("Index", "ClientHome");

            return RedirectToAction(nameof(Index));
        }
    }
}