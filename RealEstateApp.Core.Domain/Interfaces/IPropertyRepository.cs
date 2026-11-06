using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Core.Domain.Interfaces
{
    public interface IPropertyRepository : IGenericRepository<Property>
    {
        Task<List<string>> GetImagePathsByPropertyTypeIdAsync(int propertyTypeId, CancellationToken cancellationToken = default);
        Task<List<string>> GetImagePathsBySaleTypeIdAsync(int saleTypeId, CancellationToken cancellationToken = default);
    }
}
