using RealEstateApp.Core.Domain.Common.Enums;

namespace RealEstateApp.Core.Application.Dtos.Offer
{
    public class OfferDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public OfferStatus Status { get; set; }

        public string ClientId { get; set; } = string.Empty;
        public int PropertyId { get; set; }
    }
}