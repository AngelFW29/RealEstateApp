using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.ViewModels.Staff
{
    public class CreateStaffUserViewModel
    {
        [Required(ErrorMessage = "Debe completar todos los campos requeridos.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe completar todos los campos requeridos.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe completar todos los campos requeridos.")]
        public string Cedula { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe completar todos los campos requeridos.")]
        [EmailAddress(ErrorMessage = "Debe ingresar un correo electrónico válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe completar todos los campos requeridos.")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe completar todos los campos requeridos.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe completar todos los campos requeridos.")]
        [Compare("Password", ErrorMessage = "La contraseña y la confirmación de contraseña no coinciden.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
