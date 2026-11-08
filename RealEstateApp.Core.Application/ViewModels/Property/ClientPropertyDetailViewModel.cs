using RealEstateApp.Core.Application.ViewModels.Chat;
using RealEstateApp.Core.Application.ViewModels.Offer;

namespace RealEstateApp.Core.Application.ViewModels.Property
{
    public class ClientPropertyDetailViewModel
    {
        public PropertyViewModel Property { get; set; } = new();

        public List<ChatMessageViewModel> Messages { get; set; } = [];
        public List<OfferViewModel> Offers { get; set; } = [];

        public SendMessageViewModel NewMessage { get; set; } = new();
        public SendOfferViewModel NewOffer { get; set; } = new();

        public string? OfferBlockReason { get; set; }
        public bool CanSendOffer => OfferBlockReason is null;
    }
}