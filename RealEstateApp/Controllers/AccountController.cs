using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels;

namespace RealEstateApp.Controllers;

[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class AccountController : Controller
{
    private readonly IWebAccountService _accountService;
    private readonly IFileStorage _fileStorage;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        IWebAccountService accountService,
        IFileStorage fileStorage,
        ILogger<AccountController> logger)
    {
        _accountService = accountService;
        _fileStorage = fileStorage;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login() => View(new LoginViewModel());

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var result = await _accountService.LoginAsync(vm);
        if (!result.Success) { ModelState.AddModelError("", result.FirstError); return View(vm); }

        return result.Data switch
        {
            "Cliente" => RedirectToAction("Index", "ClientHome"),
            "Agente" => RedirectToAction("Index", "AgentHome"),
            "Administrador" => RedirectToAction("Index", "AdminHome"),
            _ => RedirectToAction("Index", "Home")
        };
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _accountService.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register() => View(new RegisterViewModel());

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel vm)
    {
        ModelState.Clear();
        if (!TryValidateModel(vm))
            return View(vm);

        string? photoPath = null;
        try
        {
            photoPath = await _fileStorage.SaveAsync(vm.Photo!, "users");
            _logger.LogInformation("Foto guardada en: {Path}", photoPath);

            var origin = $"{Request.Scheme}://{Request.Host}";
            var result = await _accountService.RegisterAsync(vm, photoPath, origin);

            if (!result.Success)
            {
                _fileStorage.Delete(photoPath);
                _logger.LogWarning("Registro fallido para {Email}: {Error}", vm.Email, result.FirstError);
                TempData["Error"] = result.FirstError; 
                return View(vm);
            }

            _logger.LogInformation("Usuario registrado correctamente: {Email}", vm.Email);
            TempData["Success"] = vm.UserType == "Cliente"
                ? "Su cuenta ha sido creada correctamente. Revise su correo electrónico para activar su usuario."
                : "Su cuenta de agente ha sido creada correctamente. Un administrador debe activar su usuario.";

            return RedirectToAction(nameof(Login));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en el controlador de registro para {Email}", vm.Email);
            if (photoPath != null) _fileStorage.Delete(photoPath);
            TempData["Error"] = "No fue posible completar el registro. Intente nuevamente más tarde.";
            return View(vm);
        }
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        var result = await _accountService.ConfirmEmailAsync(userId, token);
        TempData[result.Success ? "Success" : "Error"] = result.Success ? result.Message : result.FirstError;
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult AccessDenied() => View();
}