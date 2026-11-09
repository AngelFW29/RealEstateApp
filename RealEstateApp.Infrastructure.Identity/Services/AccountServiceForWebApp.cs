using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
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

    public AccountServiceForWebApp(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        IEmailService emailService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
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

        await _signInManager.SignInAsync(user, isPersistent: false);
        var role = roles.FirstOrDefault() ?? "Cliente";
        return Result<string>.Ok(role);
    }

    public async Task<Result> RegisterAsync(RegisterViewModel vm, string photoPath, string origin)
    {
        try
        {
            if (vm.UserType != "Cliente" && vm.UserType != "Agente")
                return Result.Fail("El tipo de usuario debe ser Cliente o Agente.");

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
                return Result.Fail(createResult.Errors.Select(e => e.Description).ToArray());

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
                }
                catch
                {
                    // Si el correo falla, la cuenta se crea pero se notifica al usuario
                    return Result.Fail("La cuenta fue creada pero el correo de activación falló. Contacte al administrador.");
                }
            }

            return Result.Ok();
        }
        catch
        {
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
        catch
        {
            return Result.Fail("El enlace de activación no es válido.");
        }

        user.IsActive = true;
        await _userManager.UpdateAsync(user);
        return Result.Ok("Su cuenta fue activada correctamente. Ya puede iniciar sesión.");
    }

    public async Task SignOutAsync() => await _signInManager.SignOutAsync();
}