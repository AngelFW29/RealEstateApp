using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Dtos.Agent;
using RealEstateApp.Core.Application.Dtos.Property;
using RealEstateApp.Core.Application.Interfaces;

namespace RealEstateApi.Controllers
{
    [ApiController]
    [Route("api/v1/agents")]
    [Produces("application/json")]
    public class AgentsController : ControllerBase
    {
        private readonly IAgentApiService _agentApiService;

        public AgentsController(IAgentApiService agentApiService)
        {
            _agentApiService = agentApiService;
        }

        /// <summary>
        /// Obtiene la lista de todos los agentes registrados (activos e inactivos).
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "[Administrator],[Developer]")]
        [ProducesResponseType(typeof(List<AgentApiDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> List(CancellationToken cancellationToken)
        {
            var agents = await _agentApiService.GetAllAsync(cancellationToken);
            if (agents == null || agents.Count == 0)
            {
                return NoContent();
            }
            return Ok(agents);
        }

        /// <summary>
        /// Obtiene los detalles de un agente específico por su ID.
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "[Administrator],[Developer]")]
        [ProducesResponseType(typeof(AgentApiDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(id, out _))
            {
                return BadRequest(new { Status = 400, Message = "El ID proporcionado no tiene un formato válido." });
            }

            var result = await _agentApiService.GetByIdAsync(id, cancellationToken);
            if (!result.Success)
            {
                return NotFound(new { Status = 404, Message = result.Errors.FirstOrDefault() });
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Obtiene todas las propiedades registradas por un agente específico.
        /// </summary>
        [HttpGet("{id}/properties")]
        [Authorize(Roles = "[Administrator],[Developer]")]
        [ProducesResponseType(typeof(List<PropertyDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAgentProperty(string id, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(id, out _))
            {
                return BadRequest(new { Status = 400, Message = "El ID proporcionado no tiene un formato válido." });
            }

            var result = await _agentApiService.GetAgentPropertiesAsync(id, cancellationToken);
            if (!result.Success)
            {
                return NotFound(new { Status = 404, Message = result.Errors.FirstOrDefault() });
            }

            if (result.Data == null || result.Data.Count == 0)
            {
                return NoContent();
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Activa o inactiva el estado de un agente.
        /// </summary>
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "[Administrator]")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangeStatus(string id, [FromBody] ChangeAgentStatusRequest request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(id, out _))
            {
                return BadRequest(new { Status = 400, Message = "El ID proporcionado no tiene un formato válido." });
            }

            if (request == null || !request.IsActive.HasValue)
            {
                return BadRequest(new { Status = 400, Message = "El estado enviado no es válido." });
            }

            var result = await _agentApiService.ChangeStatusAsync(id, request.IsActive.Value, cancellationToken);
            if (!result.Success)
            {
                return NotFound(new { Status = 404, Message = result.Errors.FirstOrDefault() });
            }

            return NoContent();
        }
    }
}
