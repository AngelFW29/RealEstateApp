using RealEstateApp.Core.Domain.Common.Enums;
namespace RealEstateApp.Core.Application.ViewModels.Property
{
    public class PropertyViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal SizeInMeters { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public PropertyStatus Status { get; set; }

        public string PropertyTypeName { get; set; } = string.Empty;
        public string SaleTypeName { get; set; } = string.Empty;

        public string? MainImagePath { get; set; }
        public List<string> ImagePaths { get; set; } = [];
        public List<string> ImprovementNames { get; set; } = [];

        public string AgentName { get; set; } = string.Empty;
        public string? AgentPhone { get; set; }
        public string? AgentEmail { get; set; }
        public string? AgentPhotoPath { get; set; }

        public bool IsFavorite { get; set; }
    }
}
