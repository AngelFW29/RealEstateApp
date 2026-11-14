namespace RealEstateApp.Core.Application.Dtos.Property
{
    public class PropertyApiDto
    {
        public int Id { get; set; }
        public required string Code { get; set; }
        public required string PropertyType { get; set; }
        public required string SaleType { get; set; }
        public decimal Price { get; set; }
        public decimal LandSize { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public required string Description { get; set; }
        public List<string> Improvements { get; set; } = [];
        public required string AgentName { get; set; }
        public required string AgentId { get; set; }
        public required string Status { get; set; }
    }
}