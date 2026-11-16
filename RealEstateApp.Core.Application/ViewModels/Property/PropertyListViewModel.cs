namespace RealEstateApp.Core.Application.ViewModels.Property
{
    public class PropertyListViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string PropertyTypeName { get; set; } = string.Empty;
        public string SaleTypeName { get; set; } = string.Empty;
        public string MainImageUrl { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public double Size { get; set; }
        public bool IsSold { get; set; }
    }
}
