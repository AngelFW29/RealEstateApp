namespace RealEstateApp.Core.Application.ViewModels.Offer
{
    public class AgentClientOffersViewModel
    {
        public int PropertyId { get; set; }
        public string PropertyCode { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;

        public List<OfferViewModel> Offers { get; set; } = [];
    }
}