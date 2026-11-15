using RealEstateApp.Core.Application.ViewModels.Catalog;
using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.ViewModels.Property
{
    public class SavePropertyViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Código de Propiedad")]
        public string? Code { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un tipo de propiedad.")]
        [Display(Name = "Tipo de Propiedad")]
        public int? PropertyTypeId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un tipo de venta.")]
        [Display(Name = "Tipo de Venta")]
        public int? SaleTypeId { get; set; }

        [Required(ErrorMessage = "Debe completar el precio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a cero.")]
        [Display(Name = "Precio")]
        public decimal? Price { get; set; }

        [Required(ErrorMessage = "Debe completar la descripción.")]
        [Display(Name = "Descripción")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe completar el tamaño.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El tamaño debe ser mayor a cero.")]
        [Display(Name = "Tamaño (m²)")]
        public double? Size { get; set; }

        [Required(ErrorMessage = "Debe completar la cantidad de habitaciones.")]
        [Range(0, int.MaxValue, ErrorMessage = "La cantidad de habitaciones no puede ser negativa.")]
        [Display(Name = "Habitaciones")]
        public int? Bedrooms { get; set; }

        [Required(ErrorMessage = "Debe completar la cantidad de baños.")]
        [Range(0, int.MaxValue, ErrorMessage = "La cantidad de baños no puede ser negativa.")]
        [Display(Name = "Baños")]
        public int? Bathrooms { get; set; }

        [Required(ErrorMessage = "Debe seleccionar al menos una mejora.")]
        [Display(Name = "Mejoras")]
        public List<int> ImprovementIds { get; set; } = new();

        // Listas para poblar los selectores y checkboxes en la vista
        public ICollection<CatalogListViewModel> PropertyTypes { get; set; } = new List<CatalogListViewModel>();
        public ICollection<CatalogListViewModel> SaleTypes { get; set; } = new List<CatalogListViewModel>();
        public ICollection<CatalogListViewModel> Improvements { get; set; } = new List<CatalogListViewModel>();

        public List<PropertyImageViewModel> CurrentImages { get; set; } = new();
        public List<int> ImageIdsToRemove { get; set; } = new();
    }

    public class PropertyImageViewModel
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public int Order { get; set; }
    }
}
