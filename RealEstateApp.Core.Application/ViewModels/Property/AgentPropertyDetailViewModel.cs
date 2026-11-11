using RealEstateApp.Core.Application.ViewModels.Chat;

namespace RealEstateApp.Core.Application.ViewModels.Property
{
    public class AgentPropertyDetailViewModel
    {
        public PropertyViewModel Property { get; set; } = new();
        public List<ConversationSummaryViewModel> Conversations { get; set; } = [];
        public List<ClientOffersSummaryViewModel> OfferSummaries { get; set; } = [];
    }
}
