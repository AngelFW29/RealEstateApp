using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application;
using RealEstateApp.Core.Application.Dtos.Catalog;
using RealEstateApp.Core.Application.Interfaces;

namespace RealEstateApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public abstract class CatalogApiControllerBase : ControllerBase
    {
        protected readonly ICatalogService _catalogService;

        protected abstract string NotFoundMessage { get; }

        protected CatalogApiControllerBase(ICatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        [HttpGet]
        [Authorize(Roles = "Administrator,Developer")]
        [ProducesResponseType(typeof(List<CatalogItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public virtual async Task<IActionResult> List(CancellationToken cancellationToken)
        {
            var list = await _catalogService.GetAllApiAsync(cancellationToken);
            if (list == null || list.Count == 0)
            {
                return NoContent();
            }
            return Ok(list);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator,Developer")]
        [ProducesResponseType(typeof(CatalogItemDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public virtual async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
            {
                return BadRequest(new { Status = 400, Message = "El ID proporcionado no tiene un formato válido." });
            }

            var result = await _catalogService.GetByIdApiAsync(id, cancellationToken);
            return TranslateResult(result);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(CatalogItemDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public virtual async Task<IActionResult> Create([FromBody] CatalogRequestDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Status = 400, Message = "Los datos enviados no son válidos." });
            }

            var result = await _catalogService.CreateApiAsync(dto, cancellationToken);
            if (!result.Success)
            {
                return BadRequest(new { Status = 400, Message = result.Errors.FirstOrDefault() });
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(CatalogItemDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public virtual async Task<IActionResult> Update(int id, [FromBody] CatalogRequestDto dto, CancellationToken cancellationToken)
        {
            if (id <= 0)
            {
                return BadRequest(new { Status = 400, Message = "El ID proporcionado no tiene un formato válido." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new { Status = 400, Message = "Los datos enviados no son válidos." });
            }

            var result = await _catalogService.UpdateApiAsync(id, dto, cancellationToken);
            return TranslateResult(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public virtual async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
            {
                return BadRequest(new { Status = 400, Message = "El ID proporcionado no tiene un formato válido." });
            }

            var result = await _catalogService.DeleteAsync(id, cancellationToken);
            if (!result.Success)
            {
                if (result.NotFound)
                {
                    return NotFound(new { Status = 404, Message = NotFoundMessage });
                }
                return BadRequest(new { Status = 400, Message = result.Errors.FirstOrDefault() });
            }

            return NoContent();
        }

        protected IActionResult TranslateResult<T>(Result<T> result)
        {
            if (result.Success)
            {
                return Ok(result.Data);
            }

            if (result.NotFound)
            {
                return NotFound(new { Status = 404, Message = result.Errors.FirstOrDefault() });
            }

            return BadRequest(new { Status = 400, Message = result.Errors.FirstOrDefault() });
        }
    }
}