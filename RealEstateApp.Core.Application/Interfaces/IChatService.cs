using RealEstateApp.Core.Application.Dtos.Chat;

namespace RealEstateApp.Core.Application.Interfaces
{
    public interface IChatService : IGenericService<ChatMessageDto>
    {
        Task<List<ChatMessageDto>> GetClientConversationAsync(string clientId, int propertyId);
        Task SendFromClientAsync(string clientId, int propertyId, string content);
        Task<List<ConversationSummaryDto>> GetConversationsAsync(string agentId, int propertyId);
        Task<List<ChatMessageDto>> GetAgentConversationAsync(string agentId, int propertyId, string clientId);
        Task SendFromAgentAsync(string agentId, int propertyId, string clientId, string content);
    }
}