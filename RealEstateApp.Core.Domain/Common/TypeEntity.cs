namespace RealEstateApp.Core.Domain.Common
{
    public class TypeEntity : BasicEntity<int>
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
    }
}