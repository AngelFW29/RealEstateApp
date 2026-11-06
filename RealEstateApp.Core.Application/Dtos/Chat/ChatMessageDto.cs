namespace RealEstateApp.Core.Application.Dtos.Chat
{
    public class ChatMessageDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }

        public string ClientId { get; set; } = string.Empty;
        public string AgentId { get; set; } = string.Empty;
        public string SenderId { get; set; } = string.Empty;

        public int PropertyId { get; set; }
    }
}