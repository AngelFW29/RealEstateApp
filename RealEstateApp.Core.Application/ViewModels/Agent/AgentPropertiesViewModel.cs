using RealEstateApp.Core.Application.ViewModels.Property;

namespace RealEstateApp.Core.Application.ViewModels.Agent
{
    public class AgentPropertiesViewModel
    {
        public AgentViewModel Agent { get; set; } = new();
        public PropertyFilterViewModel Filter { get; set; } = new();
        public List<PropertyViewModel> Properties { get; set; } = [];
        public string? EmptyMessage { get; set; }
    }
}