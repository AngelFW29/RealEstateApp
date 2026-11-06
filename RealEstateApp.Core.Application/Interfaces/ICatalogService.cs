using RealEstateApp.Core.Application.ViewModels.Catalog;

namespace RealEstateApp.Core.Application.Interfaces
{
    public interface ICatalogService
    {
        Task<List<CatalogListViewModel>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<SaveCatalogViewModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Result> CreateAsync(SaveCatalogViewModel vm, CancellationToken cancellationToken = default);
        Task<Result> UpdateAsync(int id, SaveCatalogViewModel vm, CancellationToken cancellationToken = default);
        Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
