using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Core.Domain.Interfaces
{
    public interface ISaleTypeRepository : ICatalogRepository<SaleType>
    {
        Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> AnyExistsAsync(CancellationToken cancellationToken = default);
    }
}
