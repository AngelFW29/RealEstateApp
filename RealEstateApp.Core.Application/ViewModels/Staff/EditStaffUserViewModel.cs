using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.ViewModels.Staff
{
    public class EditStaffUserViewModel : IValidatableObject
    {
        public string Id { get; set; } = string.Empty;

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
        public bool IsActive { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmNewPassword { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(NewPassword))
            {
                if (string.IsNullOrEmpty(ConfirmNewPassword))
                {
                    yield return new ValidationResult("Debe confirmar la nueva contraseña.", [nameof(ConfirmNewPassword)]);
                }
                else if (NewPassword != ConfirmNewPassword)
                {
                    yield return new ValidationResult("La contraseña y la confirmación de contraseña no coinciden.", [nameof(ConfirmNewPassword)]);
                }
            }
        }
    }
}
