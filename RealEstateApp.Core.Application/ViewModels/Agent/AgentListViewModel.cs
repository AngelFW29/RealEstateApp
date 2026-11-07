using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.ViewModels.Agent
{
    public class AgentListViewModel
    {
        [Display(Name = "Nombre del agente")]
        public string? SearchTerm { get; set; }
        public List<AgentViewModel> Agents { get; set; } = [];
        public string? EmptyMessage { get; set; }
    }
}