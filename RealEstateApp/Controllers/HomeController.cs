using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Dtos.Property;
using RealEstateApp.Core.Application.Exceptions;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Property;

namespace RealEstateApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPropertyService _propertyService;
        private readonly IMapper _mapper;
        public HomeController(IPropertyService propertyService, IMapper mapper)
        {
            _propertyService = propertyService;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index(PropertyFilterViewModel filter)
        {
            var model = new HomeViewModel { Filter = filter };

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

            model.EmptyMessage = BuildEmptyMessage(model);
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
        private static string? BuildEmptyMessage(HomeViewModel model)
        {
            if (model.Properties.Count > 0)
                return null;

            if (!string.IsNullOrWhiteSpace(model.Filter.Code))
                return "No se encontró ninguna propiedad disponible con el código ingresado.";

            return model.Filter.HasFilters
                ? "No se encontraron propiedades disponibles con los filtros seleccionados."
                : null;
        }        
    }
}