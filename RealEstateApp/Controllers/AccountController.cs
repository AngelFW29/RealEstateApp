using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application; 
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels;

namespace RealEstateApp.Controllers;

public class AccountController : Controller
{
    private readonly IWebAccountService _accountService;
    private readonly IFileStorage _fileStorage;

    public AccountController(IWebAccountService accountService, IFileStorage fileStorage)
    {
        _accountService = accountService;
        _fileStorage = fileStorage;
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
        if (!ModelState.IsValid) return View(vm);

        try
        {
            var photoPath = await _fileStorage.SaveAsync(vm.Photo!, "users");
            var origin = $"{Request.Scheme}://{Request.Host}";
            var result = await _accountService.RegisterAsync(vm, photoPath, origin);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.FirstError);
                return View(vm);
            }

            TempData["Success"] = vm.UserType == "Cliente"
                ? "Su cuenta ha sido creada correctamente. Revise su correo electrónico para activar su usuario."
                : "Su cuenta de agente ha sido creada correctamente. Un administrador debe activar su usuario.";

            return RedirectToAction(nameof(Login));
        }
        catch
        {
            ModelState.AddModelError("", "No fue posible completar el registro. Intente nuevamente más tarde.");
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