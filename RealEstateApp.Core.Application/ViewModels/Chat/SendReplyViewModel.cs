using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.ViewModels.Chat
{
    public class SendReplyViewModel
    {
        public int PropertyId { get; set; }

        public string ClientId { get; set; } = string.Empty;

        [Display(Name = "Mensaje")]
        [Required(ErrorMessage = "Debe escribir un mensaje antes de enviarlo.")]
        [StringLength(1000, ErrorMessage = "El mensaje no puede superar los 1000 caracteres.")]        
        public string Content { get; set; } = string.Empty;
    }
}