using RealEstateApp.Core.Domain.Common.Enums;

namespace RealEstateApp.Core.Application.ViewModels.Chat
{
    public class ClientOffersSummaryViewModel
    {
        public int PropertyId { get; set; }
        public string ClientId { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public int OffersCount { get; set; }
        public decimal LastOfferAmount { get; set; }
        public OfferStatus LastOfferStatus { get; set; }
    }
}
