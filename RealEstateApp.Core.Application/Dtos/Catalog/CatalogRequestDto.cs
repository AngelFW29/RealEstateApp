using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.Dtos.Catalog
{
    public class CatalogRequestDto
    {
        [Required(ErrorMessage = "Los datos enviados no son válidos.")]
        [StringLength(100, ErrorMessage = "Los datos enviados no son válidos.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los datos enviados no son válidos.")]
        [StringLength(500, ErrorMessage = "Los datos enviados no son válidos.")]
        public string Description { get; set; } = string.Empty;
    }
}
