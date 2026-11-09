using AutoMapper;
using RealEstateApp.Core.Application.Dtos.Offer;
using RealEstateApp.Core.Application.Exceptions;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Common.Enums;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Services
{
    public class OfferService : GenericService<OfferDto, Offer>, IOfferService
    {
        private readonly IGenericRepository<Property> _propertyRepository;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;

        public OfferService(
            IGenericRepository<Offer> repository, IGenericRepository<Property> propertyRepository,
            IUserService userService, IUnitOfWork unitOfWork,
            IMapper mapper) : base(repository, mapper)
        {
            _propertyRepository = propertyRepository;
            _userService = userService;
            _unitOfWork = unitOfWork;
        }
        public async Task<List<OfferDto>> GetClientOffersAsync(string clientId, int propertyId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(clientId);

            var offers = await _repository.GetAllAsync(o => o.ClientId == clientId && o.PropertyId == propertyId);
            return ToOrderedDtos(offers);
        }
        public async Task<string?> GetSendBlockReasonAsync(string clientId, int propertyId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(clientId);

            var property = await _propertyRepository.GetByIdAsync(propertyId);

            if (property is null || property.Status != PropertyStatus.Available || !await IsAgentActiveAsync(property.AgentId))
                return UnavailableMessage;

            var offers = await _repository.GetAllAsync(o => o.PropertyId == propertyId);

            if (offers.Any(o => o.Status == OfferStatus.Accepted))
                return AcceptedExistsMessage;

            if (offers.Any(o => o.ClientId == clientId && o.Status == OfferStatus.Pending))
                return PendingExistsMessage;

            return null;
        }
        public async Task<OfferDto> SendOfferAsync(string clientId, int propertyId, decimal amount)
        {
            if (amount <= 0)
                throw new BusinessRuleException(InvalidAmountMessage);

            var blockReason = await GetSendBlockReasonAsync(clientId, propertyId);

            if (blockReason is not null)
                throw new BusinessRuleException(blockReason);

            var offer = await _repository.AddAsync(new Offer
            {
                Id = 0,
                Amount = amount,
                ClientId = clientId,
                PropertyId = propertyId,
                Status = OfferStatus.Pending,
                CreatedAt = DateTime.UtcNow
            });

            return _mapper.Map<OfferDto>(offer);
        }
        public async Task<List<ClientOffersSummaryDto>> GetClientSummariesAsync(string agentId, int propertyId)
        {
            await GetOwnedPropertyAsync(agentId, propertyId);

            var offers = await _repository.GetAllAsync(o => o.PropertyId == propertyId);

            if (offers.Count == 0)
                return [];

            var groups = offers
                .GroupBy(o => o.ClientId)
                .Select(g => (Last: g.OrderByDescending(o => o.CreatedAt).ThenByDescending(o => o.Id).First(), Count: g.Count()))
                .OrderByDescending(x => x.Last.CreatedAt)
                .ToList();

            var clients = await _userService.GetBasicInfoByIdsAsync(groups.Select(x => x.Last.ClientId));

            return groups.Select(x => new ClientOffersSummaryDto
            {
                PropertyId = propertyId,
                ClientId = x.Last.ClientId,
                ClientName = clients.TryGetValue(x.Last.ClientId, out var client) ? client.FullName : string.Empty,
                OffersCount = x.Count,
                LastOfferAmount = x.Last.Amount,
                LastOfferStatus = x.Last.Status
            }).ToList();
        }
        public async Task<List<OfferDto>> GetOffersByClientAsync(string agentId, int propertyId, string clientId)
        {
            await GetOwnedPropertyAsync(agentId, propertyId);

            var offers = await _repository.GetAllAsync(o => o.PropertyId == propertyId && o.ClientId == clientId);
            return ToOrderedDtos(offers);
        }
        public async Task RejectOfferAsync(string agentId, int propertyId, int offerId)
        {
            await GetOwnedPropertyAsync(agentId, propertyId);

            var offer = await GetOfferOfPropertyAsync(propertyId, offerId);

            if (offer.Status != OfferStatus.Pending)
                throw new BusinessRuleException(AlreadyAnsweredMessage);

            offer.Status = OfferStatus.Rejected;
            await _repository.UpdateAsync(offer);
        }
        public async Task AcceptOfferAsync(string agentId, int propertyId, int offerId)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var property = await GetOwnedPropertyAsync(agentId, propertyId);
                var offer = await GetOfferOfPropertyAsync(propertyId, offerId);

                if (offer.Status != OfferStatus.Pending)
                    throw new BusinessRuleException(AlreadyAnsweredMessage);

                var propertyOffers = await _repository.GetAllAsync(o => o.PropertyId == propertyId);

                if (property.Status != PropertyStatus.Available || propertyOffers.Any(o => o.Status == OfferStatus.Accepted))
                    throw new BusinessRuleException(PropertySoldMessage);

                offer.Status = OfferStatus.Accepted;
                await _repository.UpdateAsync(offer);

                foreach (var other in propertyOffers.Where(o => o.Id != offerId && o.Status == OfferStatus.Pending))
                {
                    other.Status = OfferStatus.Rejected;
                    await _repository.UpdateAsync(other);
                }

                property.Status = PropertyStatus.Sold;
                await _propertyRepository.UpdateAsync(property);
            });
        }

        #region Private methods
        // Message
        private const string UnavailableMessage = "Esta propiedad ya no se encuentra disponible para recibir ofertas.";
        private const string AcceptedExistsMessage = "Esta propiedad ya tiene una oferta aceptada y no permite nuevas ofertas.";
        private const string PendingExistsMessage = "Ya tiene una oferta pendiente para esta propiedad.";
        private const string InvalidAmountMessage = "El monto de la oferta debe ser un valor numérico mayor que cero.";
        private const string NotOwnerMessage = "No tiene permisos para gestionar las ofertas de esta propiedad.";
        private const string OfferNotFoundMessage = "La oferta solicitada no existe.";
        private const string AlreadyAnsweredMessage = "Esta oferta ya fue respondida.";
        private const string PropertySoldMessage = "No se puede aceptar una oferta para una propiedad que ya fue vendida.";

        // Methods
        private async Task<Property> GetOwnedPropertyAsync(string agentId, int propertyId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(agentId);

            var property = await _propertyRepository.GetByIdAsync(propertyId);

            if (property is null || property.AgentId != agentId)
                throw new BusinessRuleException(NotOwnerMessage);

            return property;
        }
        private async Task<Offer> GetOfferOfPropertyAsync(int propertyId, int offerId)
        {
            var offer = await _repository.GetByIdAsync(offerId);

            if (offer is null || offer.PropertyId != propertyId)
                throw new BusinessRuleException(OfferNotFoundMessage);

            return offer;
        }
        private async Task<bool> IsAgentActiveAsync(string agentId)
        {
            var activeAgentIds = await _userService.GetActiveAgentIdsAsync();
            return activeAgentIds.Contains(agentId);
        }
        private List<OfferDto> ToOrderedDtos(IEnumerable<Offer> offers)
        {
            var ordered = offers.OrderByDescending(o => o.CreatedAt).ThenByDescending(o => o.Id);
            return _mapper.Map<List<OfferDto>>(ordered);
        }
        #endregion
    }
}