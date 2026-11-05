using RealEstateApp.Core.Domain.Common;

namespace RealEstateApp.Core.Domain.Entities
{
    public class ChatMessage : BasicEntity<int>
    {
        public required string Content { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public required string ClientId { get; set; }
        public required string AgentId { get; set; }
        public required string SenderId { get; set; }

        public int PropertyId { get; set; }
        public Property? Property { get; set; }
    }
}