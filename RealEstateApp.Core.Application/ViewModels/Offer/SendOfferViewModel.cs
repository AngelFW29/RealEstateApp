using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.ViewModels.Offer
{
    public class SendOfferViewModel
    {
        public int PropertyId { get; set; }

        [Display(Name = "Mensaje ofertado")]
        [Required(ErrorMessage = "Debe ingresar el monto de la oferta.")]
        [Range(0.01, 9999999999999999, ErrorMessage = "El monto de la oferta debe ser un valor numérico mayor que cero.")]
        public decimal? Amount { get; set; }
    }
}