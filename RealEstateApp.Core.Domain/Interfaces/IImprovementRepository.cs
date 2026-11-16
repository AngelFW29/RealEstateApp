using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Core.Domain.Interfaces
{
    public interface IImprovementRepository : ICatalogRepository<Improvement>
    {
        Task<bool> AnyExistsAsync(CancellationToken cancellationToken = default);
    }
}