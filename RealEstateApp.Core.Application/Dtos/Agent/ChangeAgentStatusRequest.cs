using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RealEstateApp.Core.Application.Dtos.Agent
{
    public class ChangeAgentStatusRequest
    {
        [Required(ErrorMessage = "El estado enviado no es válido.")]
        [JsonPropertyName("isActive")]
        public bool? IsActive { get; set; }
    }
}
