using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.ViewModels.Catalog
{
    public class SaveCatalogViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe completar todos los campos requeridos.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe completar todos los campos requeridos.")]
        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres.")]
        public string Description { get; set; } = string.Empty;
    }
}
