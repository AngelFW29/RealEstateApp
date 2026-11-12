namespace RealEstateApp.Core.Application.ViewModels.Chat
{
    public class ConversationSummaryViewModel
    {
        private const int PreviewLength = 60;

        public int PropertyId { get; set; }
        public string ClientId { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string LastMessage { get; set; } = string.Empty;
        public DateTime LastMessageAt { get; set; }

        public string LastMessagePreview => LastMessage.Length > PreviewLength
            ? LastMessage[..PreviewLength] + "…"
            : LastMessage;
    }
}