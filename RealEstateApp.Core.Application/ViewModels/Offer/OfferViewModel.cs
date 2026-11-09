using RealEstateApp.Core.Domain.Common.Enums;

namespace RealEstateApp.Core.Application.ViewModels.Offer
{
    public class OfferViewModel
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public OfferStatus Status { get; set; }
        public string StatusText => Status switch
        {
            OfferStatus.Pending => "Pendiente",
            OfferStatus.Rejected => "Rechazada",
            OfferStatus.Accepted => "Aceptada",
            _ => string.Empty
        };
    }
}