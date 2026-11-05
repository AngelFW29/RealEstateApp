using RealEstateApp.Core.Domain.Common;

namespace RealEstateApp.Core.Domain.Entities
{
    public class Favorite : BasicEntity<int>
    {
        public required String ClientId { get; set; }

        public int PropertyId { get; set; }
        public Property? Property { get; set; }
    }
}