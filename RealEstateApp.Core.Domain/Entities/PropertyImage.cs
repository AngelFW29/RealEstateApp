using RealEstateApp.Core.Domain.Common;

namespace RealEstateApp.Core.Domain.Entities
{
    public class PropertyImage : BasicEntity<int>
    {
        public required string ImagePath { get; set; }
        public bool IsMain { get; set; }
        public int DisplayOrder { get; set; }

        public int PropertyId { get; set; }
        public Property? Property { get; set; }
    }
}