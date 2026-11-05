namespace RealEstateApp.Core.Domain.Common
{
    public class BasicEntity<Tkey>
    {
        public required Tkey Id { get; set; }
    }
}