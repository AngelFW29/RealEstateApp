using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.ViewModels.Agent
{
    public class AgentProfileViewModel
    {
        [Required(ErrorMessage = "Debe completar todos los campos requeridos.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe completar todos los campos requeridos.")]
        public string PhoneNumber { get; set; } = string.Empty;

        public string? CurrentPhotoUrl { get; set; }
    }
}
