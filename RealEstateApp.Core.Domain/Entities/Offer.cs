using RealEstateApp.Core.Domain.Common;
using RealEstateApp.Core.Domain.Common.Enums;

namespace RealEstateApp.Core.Domain.Entities
{
    public class Offer : BasicEntity<int>
    {
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public OfferStatus Status { get; set; } = OfferStatus.Pending;

        public required string ClientId { get; set; }

        public int PropertyId { get; set; }
        public Property? Property { get; set; }
    }
}