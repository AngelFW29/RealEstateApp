using RealEstateApp.Core.Domain.Common.Enums;

namespace RealEstateApp.Core.Application.Dtos.Property
{
    public class PropertyListDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string PropertyTypeName { get; set; } = string.Empty;
        public string SaleTypeName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal SizeInMeters { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public string MainImagePath { get; set; } = string.Empty;
        public PropertyStatus Status { get; set; }
    }
}
