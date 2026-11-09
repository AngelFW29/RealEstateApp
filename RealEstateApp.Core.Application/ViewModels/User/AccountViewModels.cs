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

    public class RegisterViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "El nombre es requerido.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es requerido.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe ingresar su teléfono.")]
        [StringLength(12, MinimumLength = 12, ErrorMessage = "El teléfono debe tener el formato 809-555-1234.")]
        [RegularExpression(@"^(809|829|849)-\d{3}-\d{4}$", ErrorMessage = "Debe ingresar un número telefónico válido de República Dominicana.")]
        [Display(Name = "Teléfono")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "La foto es requerida.")]
        public IFormFile? Photo { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es requerido.")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es requerido.")]
        [EmailAddress(ErrorMessage = "Formato inválido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "La confirmación es requerida.")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "No coinciden.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de usuario es requerido.")]
        public string? UserType { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Photo == null || Photo.Length == 0)
            {
                yield return new ValidationResult(
                    "La foto de perfil es requerida.",
                    new[] { nameof(Photo) });
            }
            else
            {
                bool isValid = false;
                try
                {
                    isValid = ImageHelper.IsValid(Photo);
                }
                catch
                {
                    isValid = false;
                }

                if (!isValid)
                {
                    yield return new ValidationResult(
                        "El archivo seleccionado no tiene un formato de imagen válido.",
                        new[] { nameof(Photo) });
                }
            }
        }
    }
}