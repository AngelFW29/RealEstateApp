using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Dtos;
using RealEstateApp.Core.Application.Interfaces;

namespace RealEstateApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class AccountController : ControllerBase
{
    private readonly IApiAccountService _accountService;
    public AccountController(IApiAccountService accountService) => _accountService = accountService;

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _accountService.AuthenticateAsync(dto);
        if (result.StatusCode == 200) return Ok(result.Data);
        return StatusCode(result.StatusCode, new { Status = result.StatusCode, Message = result.Message });
    }

    [HttpPost("register-developer")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> RegisterDeveloper([FromBody] RegisterUserDto dto) => await Register(dto, "Desarrollador");

    [HttpPost("register-administrator")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> RegisterAdministrator([FromBody] RegisterUserDto dto) => await Register(dto, "Administrador");

    private async Task<IActionResult> Register(RegisterUserDto dto, string role)
    {
        var result = await _accountService.RegisterAsync(dto, role);
        if (!result.Success) return BadRequest(new { Status = 400, Message = result.FirstError, Errors = result.Errors });
        return StatusCode(201, new { Message = "Usuario creado correctamente." });
    }
}