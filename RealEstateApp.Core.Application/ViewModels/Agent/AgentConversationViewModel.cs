using RealEstateApp.Core.Application.ViewModels.Chat;

namespace RealEstateApp.Core.Application.ViewModels.Agent
{
    public class AgentConversationViewModel
    {
        public int PropertyId { get; set; }
        public string PropertyCode { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;

        public List<ChatMessageViewModel> Messages { get; set; } = [];
        public SendReplyViewModel Reply { get; set; } = new();
    }
}