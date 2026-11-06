using RealEstateApp.Core.Domain.Common;

namespace RealEstateApp.Core.Domain.Interfaces
{
    public interface ICatalogRepository<T> : IGenericRepository<T> where T : TypeEntity
    {
        Task<bool> ExistsByNameAsync(string nombre, int? excludeId = null);
        Task<List<(T Item, int Cantidad)>> GetAllWithCountAsync(CancellationToken cancellationToken = default);
    }
}
