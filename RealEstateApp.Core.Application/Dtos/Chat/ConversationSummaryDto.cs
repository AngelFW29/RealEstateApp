namespace RealEstateApp.Core.Application.Dtos.Chat
{
    public class ConversationSummaryDto
    {
        public int PropertyId { get; set; }
        public string ClientId { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string LastMessage { get; set; } = string.Empty;
        public DateTime LastMessageAt { get; set; }
    }
}