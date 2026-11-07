using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Core.Domain.Interfaces
{
    public interface IPropertyTypeRepository : ICatalogRepository<PropertyType>
    {
        Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> AnyExistsAsync(CancellationToken cancellationToken = default);
    }
}
