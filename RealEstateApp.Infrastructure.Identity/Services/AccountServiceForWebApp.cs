using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Text;
using RealEstateApp.Core.Application;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels;
using RealEstateApp.Infrastructure.Identity.Entities;

namespace RealEstateApp.Infrastructure.Identity.Services;

public class AccountServiceForWebApp : IWebAccountService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IEmailService _emailService;
    private readonly ILogger<AccountServiceForWebApp> _logger;

    public AccountServiceForWebApp(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        IEmailService emailService,
        ILogger<AccountServiceForWebApp> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<Result<string>> LoginAsync(LoginViewModel vm)
    {
        var user = await _userManager.FindByEmailAsync(vm.UserNameOrEmail)
                   ?? await _userManager.FindByNameAsync(vm.UserNameOrEmail);

        if (user == null || !await _userManager.CheckPasswordAsync(user, vm.Password))
            return Result<string>.Fail("Los datos de acceso son inválidos.");

        if (!user.IsActive)
            return Result<string>.Fail("El usuario se encuentra inactivo y no puede iniciar sesión.");

        var roles = await _userManager.GetRolesAsync(user);

        // Un Desarrollador NO puede entrar a la WebApp
        if (roles.Contains("Desarrollador"))
            return Result<string>.Fail("No tiene permisos para acceder a la aplicación web. Su usuario solo puede utilizar la API.");

        var role = roles.FirstOrDefault();
        if (string.IsNullOrEmpty(role))
            return Result<string>.Fail("El usuario no tiene un rol válido asignado. Contacte a un administrador.");

        await _signInManager.SignInAsync(user, isPersistent: false);
        return Result<string>.Ok(role);
    }

    public async Task<Result> RegisterAsync(RegisterViewModel vm, string photoPath, string origin)
    {
        try
        {
            if (vm.UserType != "Cliente" && vm.UserType != "Agente")
                return Result.Fail("El tipo de usuario debe ser Cliente o Agente.");

            // Validación de duplicados
            var existingEmail = await _userManager.FindByEmailAsync(vm.Email);
            if (existingEmail != null)
                return Result.Fail("Ya existe un usuario registrado con este correo electrónico.");

            var existingName = await _userManager.FindByNameAsync(vm.UserName);
            if (existingName != null)
                return Result.Fail("Ya existe un usuario registrado con este nombre de usuario.");

            var user = new AppUser
            {
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                PhoneNumber = vm.Phone,
                UserName = vm.UserName,
                Email = vm.Email,
                PhotoPath = photoPath,
                IsActive = false,
                EmailConfirmed = false
            };

            var createResult = await _userManager.CreateAsync(user, vm.Password);
            if (!createResult.Succeeded)
            {
                var errors = createResult.Errors.Select(e => e.Description).ToArray();
                _logger.LogWarning("Error al crear usuario {Email}: {Errors}", vm.Email, string.Join(", ", errors));
                return Result.Fail(errors);
            }

            await _userManager.AddToRoleAsync(user, vm.UserType!);

            if (vm.UserType == "Cliente")
            {
                try
                {
                    var token = WebEncoders.Base64UrlEncode(
                        Encoding.UTF8.GetBytes(await _userManager.GenerateEmailConfirmationTokenAsync(user)));
                    var link = $"{origin}/Account/ConfirmEmail?userId={user.Id}&token={token}";

                    await _emailService.SendAsync(
                        user.Email!,
                        "Activación de cuenta en RealEstateApp",
                        $"<p>Hola {user.FirstName},</p><p>Para activar su usuario, haga clic aquí: <a href='{link}'>Activar cuenta</a></p>");

                    _logger.LogInformation("Correo de activación enviado a {Email}", user.Email);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al enviar correo de activación a {Email}", user.Email);
                    await _userManager.DeleteAsync(user);
                    return Result.Fail("No fue posible enviar el correo de activación. Intente nuevamente más tarde.");
                }
            }

            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado en RegisterAsync para {Email}", vm.Email);
            return Result.Fail("No fue posible completar el registro. Intente nuevamente.");
        }
    }

    public async Task<Result> ConfirmEmailAsync(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return Result.Fail("El enlace de activación no es válido.");

        try
        {
            var decoded = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userManager.ConfirmEmailAsync(user, decoded);
            if (!result.Succeeded) return Result.Fail("El enlace de activación no es válido o ya expiró.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al confirmar email del usuario {UserId}", userId);
            return Result.Fail("El enlace de activación no es válido.");
        }

        user.IsActive = true;
        await _userManager.UpdateAsync(user);
        _logger.LogInformation("Usuario {UserId} activado correctamente", userId);
        return Result.Ok("Su cuenta fue activada correctamente. Ya puede iniciar sesión.");
    }

    public async Task SignOutAsync() => await _signInManager.SignOutAsync();
}