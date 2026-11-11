using AutoMapper;
using RealEstateApp.Core.Application.Dtos.Chat;
using RealEstateApp.Core.Application.Exceptions;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Common.Enums;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Services
{
    public class ChatService : GenericService<ChatMessageDto, ChatMessage>, IChatService
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IUserService _userService;
        public ChatService(IChatMessageRepository repository,
            IPropertyRepository propertyRepository, IUserService userService,
            IMapper mapper) : base(repository, mapper)
        {
            _propertyRepository = propertyRepository;
            _userService = userService;
        }
        public async Task<List<ChatMessageDto>> GetClientConversationAsync(string clientId, int propertyId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(clientId);

            var messages = await _repository.GetAllAsync(m => m.PropertyId == propertyId && m.ClientId == clientId);
            return ToOrderedDtos(messages);
        }
        public async Task SendFromClientAsync(string clientId, int propertyId, string content)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(clientId);
            var text = NormalizeContent(content);

            var property = await _propertyRepository.GetByIdAsync(propertyId);

            if (property is null || property.Status != PropertyStatus.Available || !await IsAgentActiveAsync(property.AgentId))
                throw new BusinessRuleException(UnavailableMessage);

            await SaveMessageAsync(propertyId, clientId, property.AgentId, clientId, text);
        }
        public async Task<List<ConversationSummaryDto>> GetConversationsAsync(string agentId, int propertyId)
        {
            await EnsureOwnedPropertyAsync(agentId, propertyId);

            var messages = await _repository.GetAllAsync(m => m.PropertyId == propertyId);

            if (messages.Count == 0)
                return [];

            var lastMessages = messages
                .GroupBy(m => m.ClientId)
                .Select(g => g.OrderByDescending(m => m.SentAt).ThenByDescending(m => m.Id).First())
                .OrderByDescending(m => m.SentAt)
                .ToList();

            var clients = await _userService.GetBasicInfoByIdsAsync(lastMessages.Select(m => m.ClientId));

            return lastMessages.Select(m => new ConversationSummaryDto
            {
                PropertyId = propertyId,
                ClientId = m.ClientId,
                ClientName = clients.TryGetValue(m.ClientId, out var client) ? client.FullName : string.Empty,
                LastMessage = m.Content,
                LastMessageAt = m.SentAt
            }).ToList();
        }
        public async Task<List<ChatMessageDto>> GetAgentConversationAsync(string agentId, int propertyId, string clientId)
        {
            await EnsureOwnedPropertyAsync(agentId, propertyId);
            ArgumentException.ThrowIfNullOrWhiteSpace(clientId);

            var messages = await _repository.GetAllAsync(m => m.PropertyId == propertyId && m.ClientId == clientId);

            if (messages.Count == 0)
                throw new BusinessRuleException(ConversationNotFoundMessage);

            return ToOrderedDtos(messages);
        }
        public async Task SendFromAgentAsync(string agentId, int propertyId, string clientId, string content)
        {
            await EnsureOwnedPropertyAsync(agentId, propertyId);
            ArgumentException.ThrowIfNullOrWhiteSpace(clientId);
            var text = NormalizeContent(content);

            var hasConversation = await _repository.CountAsync(m => m.PropertyId == propertyId && m.ClientId == clientId) > 0;

            if (!hasConversation)
                throw new BusinessRuleException(ConversationNotFoundMessage);

            await SaveMessageAsync(propertyId, clientId, agentId, agentId, text);
        }

        #region Private methods & const
        private const int MaxContentLength = 1000;
        private const string EmptyMessage = "Debe escribir un mensaje antes de enviarlo.";
        private const string TooLongMessage = "El mensaje no puede superar los 1000 caracteres.";
        private const string UnavailableMessage = "La propiedad solicitada no existe o no se encuentra disponible.";
        private const string NotOwnerMessage = "No tiene permisos para gestionar las conversaciones de esta propiedad.";
        private const string ConversationNotFoundMessage = "La conversación solicitada no existe.";

        // Methods
        private async Task SaveMessageAsync(int propertyId, string clientId, string agentId, string senderId, string content)
        {
            await _repository.AddAsync(new ChatMessage
            {
                Id = 0,
                Content = content,
                SentAt = DateTime.UtcNow,
                ClientId = clientId,
                AgentId = agentId,
                SenderId = senderId,
                PropertyId = propertyId
            });
        }
        private static string NormalizeContent(string? content)
        {
            var text = content?.Trim();

            if (string.IsNullOrEmpty(text))
                throw new BusinessRuleException(EmptyMessage);

            if (text.Length > MaxContentLength)
                throw new BusinessRuleException(TooLongMessage);

            return text;
        }
        private async Task EnsureOwnedPropertyAsync(string agentId, int propertyId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(agentId);

            var property = await _propertyRepository.GetByIdAsync(propertyId);

            if (property is null || property.AgentId != agentId)
                throw new BusinessRuleException(NotOwnerMessage);
        }
        private async Task<bool> IsAgentActiveAsync(string agentId)
        {
            var activeAgentIds = await _userService.GetActiveAgentIdsAsync();
            return activeAgentIds.Contains(agentId);
        }
        private List<ChatMessageDto> ToOrderedDtos(IEnumerable<ChatMessage> messages)
        {
            var ordered = messages.OrderBy(m => m.SentAt).ThenBy(m => m.Id);
            return _mapper.Map<List<ChatMessageDto>>(ordered);
        }
        #endregion
    }
}