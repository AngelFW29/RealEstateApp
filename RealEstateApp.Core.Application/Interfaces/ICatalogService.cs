using RealEstateApp.Core.Application.Dtos.Catalog;
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
        Task<List<CatalogItemDto>> GetAllApiAsync(CancellationToken cancellationToken = default);
        Task<Result<CatalogItemDto>> GetByIdApiAsync(int id, CancellationToken cancellationToken = default);
        Task<Result<CatalogItemDto>> UpdateApiAsync(int id, CatalogRequestDto dto, CancellationToken cancellationToken = default);
        Task<Result<CatalogItemDto>> CreateApiAsync(CatalogRequestDto dto, CancellationToken cancellationToken = default);
    }
}
