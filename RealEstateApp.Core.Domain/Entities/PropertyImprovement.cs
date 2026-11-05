using RealEstateApp.Core.Domain.Common;

namespace RealEstateApp.Core.Domain.Entities
{
    public class PropertyImprovement : BasicEntity<int>
    {
        public int PropertyId { get; set; }
        public Property? Property { get; set; }

        public int ImprovementId { get; set; }
        public Improvement? Improvement { get; set; }
    }
}