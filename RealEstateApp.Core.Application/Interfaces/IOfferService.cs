using RealEstateApp.Core.Application.Dtos.Offer;

namespace RealEstateApp.Core.Application.Interfaces
{
    public interface IOfferService : IGenericService<OfferDto>
    {
		Task<List<OfferDto>> GetClientOffersAsync(string clientId, int propertyId);
		Task<string?> GetSendBlockReasonAsync(string clientId, int propertyId);
		Task<OfferDto> SendOfferAsync(string clientId, int propertyId, decimal amount);
		Task<List<ClientOffersSummaryDto>> GetClientSummariesAsync(string agentId, int propertyId);
		Task<List<OfferDto>> GetOffersByClientAsync(string agentId, int propertyId, string clientId);
		Task AcceptOfferAsync(string agentId, int propertyId, int offerId);
		Task RejectOfferAsync(string agentId, int propertyId, int offerId);
	}
}