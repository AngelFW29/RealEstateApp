using RealEstateApp.Core.Domain.Common;
using RealEstateApp.Core.Domain.Common.Enums;

namespace RealEstateApp.Core.Domain.Entities
{
    public class Property : BasicEntity<int>
    {
        public required string Code { get; set; }
        public required string Description { get; set; }
        public decimal Price { get; set; }
        public decimal SizeInMeters { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public PropertyStatus Status { get; set; } = PropertyStatus.Available;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public required string AgentId { get; set; }

        #region Navigation properties
        // navigation properties
        public int PropertyTypeId { get; set; }
        public PropertyType? PropertyType { get; set; }

        public int SaleTypeId { get; set; }
        public SaleType? SaleType { get; set; }
        #endregion

        #region Collections
        public ICollection<PropertyImage> Images { get; set; } = [];
        public ICollection<PropertyImprovement> Improvements { get; set; } = [];
        public ICollection<Favorite> Favorites { get; set; } = [];
        public ICollection<ChatMessage> Messages { get; set; } = [];
        public ICollection<Offer> Offers { get; set; } = [];
        #endregion
    }
}
