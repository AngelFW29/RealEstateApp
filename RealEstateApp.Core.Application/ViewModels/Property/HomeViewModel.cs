namespace RealEstateApp.Core.Application.ViewModels.Property
{
    public class HomeViewModel
    {
        public List<PropertyViewModel> Properties { get; set; } = [];
        public PropertyFilterViewModel Filter { get; set; } = new();
        public string? EmptyMessage { get; set; }
    }
}