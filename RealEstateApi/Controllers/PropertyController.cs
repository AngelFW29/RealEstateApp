using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Dtos.Property;
using RealEstateApp.Core.Application.Interfaces;

namespace RealEstateApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    [Authorize(Roles = "Administrador,Desarrollador")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public class PropertyController : Controller
    {
        private readonly IPropertyService _propertyService;
        private readonly ILogger<PropertyController> _logger;
        public PropertyController(IPropertyService propertyService, ILogger<PropertyController> logger)
        {
            _propertyService = propertyService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PropertyApiDto>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> List()
        {
            try
            {
                var properties = await _propertyService.GetAllForApiAsync();

                if (properties.Count == 0)
                    return NoContent();

                return Ok(properties);
            }
            catch (Exception ex)
            {
                return InternalError(ex);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PropertyApiDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                if (!int.TryParse(id, out var propertyId) || propertyId <= 0)
                    return Error(StatusCodes.Status400BadRequest, "El Id enviado no tiene un formato válido.");

                var property = await _propertyService.GetByIdForApiAsync(propertyId);

                if (property is null)
                    return Error(StatusCodes.Status404NotFound, "La propiedad solicitada no existe.");

                return Ok(property);
            }
            catch (Exception ex)
            {
                return InternalError(ex);
            }
        }

        [HttpGet("code/{code}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PropertyApiDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByCode(string code)
        {
            try
            {
                if (!IsValidCode(code))
                    return Error(StatusCodes.Status400BadRequest, "El código enviado no tiene un formato válido.");

                var property = await _propertyService.GetByCodeForApiAsync(code);

                if (property is null)
                    return Error(StatusCodes.Status404NotFound, "No existe una propiedad registrada con el código enviado.");

                return Ok(property);
            }
            catch (Exception ex)
            {
                return InternalError(ex);
            }
        }

        #region Private methods
        private static bool IsValidCode(string code) => code.Length == 6 && code.All(char.IsAsciiDigit);
        private IActionResult Error(int status, string message) =>
            StatusCode(status, new { Status = status, Message = message });
        private IActionResult InternalError(Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in PropertyController.");
            return Error(StatusCodes.Status500InternalServerError, InternalErrorMessage);
        }
        private const string InternalErrorMessage = "Ocurrió un error interno en el servidor.";
        #endregion
    }
}