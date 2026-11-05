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
        [Range(0, double.MaxValue, ErrorMessage = "El precio mínimo no puede ser menor que cero.")]
        public decimal? MinPrice { get; set; }

        [Display(Name = "Precio máximo")]
        [Range(0, double.MaxValue, ErrorMessage = "El precio máximo no puede ser menor que cero.")]
        public decimal? MaxPrice { get; set; }

        [Display(Name = "Habitaciones")]
        [Range(0, int.MaxValue, ErrorMessage = "La cantidad de habitaciones no puede ser menor que cero.")]
        public int? Bedrooms { get; set; }

        [Display(Name = "Baños")]
        [Range(0, int.MaxValue, ErrorMessage = "La cantidad de baños no puede ser menor que cero.")]
        public int? Bathrooms { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (MinPrice > MaxPrice)
                yield return new ValidationResult(
                    "El precio mínimo no puede ser mayor que el precio máximo.",
                    [nameof(MinPrice), nameof(MaxPrice)]);
        }
    }
}