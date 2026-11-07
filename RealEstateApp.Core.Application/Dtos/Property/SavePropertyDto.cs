namespace RealEstateApp.Core.Application.Dtos.Property
{
    public class SavePropertyDto
    {
        public int Id { get; set; }
        public int PropertyTypeId { get; set; }
        public int SaleTypeId { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal SizeInMeters { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }

        // Colecciones de IDs para mejoras e imágenes nuevas/eliminadas
        public List<int> SelectedImprovementIds { get; set; } = [];
        public List<ImageUploadDto> NewImages { get; set; } = [];
        public List<PropertyImageDto> CurrentImages { get; set; } = [];
        public List<int> ImageIdsToRemove { get; set; } = [];
    }
}
