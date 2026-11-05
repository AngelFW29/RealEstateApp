using RealEstateApp.Core.Domain.Common.Enums;


namespace RealEstateApp.Core.Application.Dtos.Property
{
    public class PropertyFilterDto
    {
        public string? Code { get; set; }
        public int? PropertyTypeId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? Bedrooms { get; set; }
        public int? Bathrooms { get; set; }
        public string? AgentId { get; set; }
    }
}