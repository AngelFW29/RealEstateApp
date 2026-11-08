namespace RealEstateApp.Core.Application.ViewModels.Chat
{
    public class ChatMessageViewModel
    {
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public string ClientId { get; set; } = string.Empty;
        public string SenderId { get; set; } = string.Empty;

        public bool IsFromClient => SenderId == ClientId;
        public string SenderText => IsFromClient ? "Cliente" : "Agente";
    }
}