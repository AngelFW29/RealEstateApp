using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace RealEstateApp.Core.Application.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Debe ingresar su correo o nombre de usuario y contraseña.")]
        public string UserNameOrEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe ingresar su correo o nombre de usuario y contraseña.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "El nombre es requerido.")] public string FirstName { get; set; } = string.Empty;
        [Required(ErrorMessage = "El apellido es requerido.")] public string LastName { get; set; } = string.Empty;
        [Required(ErrorMessage = "El teléfono es requerido.")] public string Phone { get; set; } = string.Empty;
        [Required(ErrorMessage = "La foto es requerida.")] public IFormFile? Photo { get; set; }
        [Required(ErrorMessage = "El nombre de usuario es requerido.")] public string UserName { get; set; } = string.Empty;
        [Required(ErrorMessage = "El correo es requerido."), EmailAddress(ErrorMessage = "Formato inválido.")] public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "La contraseña es requerida."), DataType(DataType.Password)] public string Password { get; set; } = string.Empty;
        [Required(ErrorMessage = "La confirmación es requerida."), DataType(DataType.Password), Compare(nameof(Password), ErrorMessage = "No coinciden.")] public string ConfirmPassword { get; set; } = string.Empty;
        [Required(ErrorMessage = "El tipo de usuario es requerido.")] public string? UserType { get; set; }
    }
}