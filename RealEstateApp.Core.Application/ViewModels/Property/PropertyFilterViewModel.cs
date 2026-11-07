using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.ViewModels.Property
{
    public class PropertyFilterViewModel : IValidatableObject
    {
        [Display(Name = "Código de propiedad")]
        public string? Code { get; set; }

        [Display(Name = "Tipo de propiedad")]
        public int? PropertyTypeId { get; set; }

        [Display(Name = "Precio mínimo")]
        [Range(typeof(decimal), "0", "9999999999999999.99",
            ErrorMessage = "El precio mínimo no puede ser menor que cero.")]
        public decimal? MinPrice { get; set; }

        [Display(Name = "Precio máximo")]
        [Range(typeof(decimal), "0", "9999999999999999.99",
            ErrorMessage = "El precio máximo no puede ser menor que cero.")]
        public decimal? MaxPrice { get; set; }

        [Display(Name = "Habitaciones")]
        [Range(0, int.MaxValue, ErrorMessage = "La cantidad de habitaciones no puede ser menor que cero.")]
        public int? Bedrooms { get; set; }

        [Display(Name = "Baños")]
        [Range(0, int.MaxValue, ErrorMessage = "La cantidad de baños no puede ser menor que cero.")]
        public int? Bathrooms { get; set; }
        public bool HasFilters => PropertyTypeId.HasValue || MinPrice.HasValue || MaxPrice.HasValue
                          || Bedrooms.HasValue || Bathrooms.HasValue;
        public string? GetNoResultsMessage()
        {
            if (!string.IsNullOrWhiteSpace(Code))
                return "No se encontró ninguna propiedad disponible con el código ingresado.";

            return HasFilters
                ? "No se encontraron propiedades disponibles con los filtros seleccionados."
                : null;
        }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (MinPrice > MaxPrice)
                yield return new ValidationResult(
                    "El precio mínimo no puede ser mayor que el precio máximo.",
                    [nameof(MinPrice), nameof(MaxPrice)]);
        }
    }
}