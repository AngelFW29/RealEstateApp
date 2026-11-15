using RealEstateApp.Core.Application.Dtos.Property;
using RealEstateApp.Core.Application.ViewModels.Property;

namespace RealEstateApp.Core.Application.Interfaces
{
    public interface IPropertyService : IGenericService<PropertyDto>
    {
        Task<List<PropertyDto>> GetAvailableAsync(PropertyFilterDto filter);
        Task<PropertyDto?> GetAvailableByIdAsync(int id);
        Task<List<PropertyDto>> GetByAgentAsync(string agentId, PropertyFilterDto filter);
        Task<PropertyDto?> GetAgentPropertyByIdAsync(string agentId, int id);
        Task<List<PropertyListViewModel>> GetAllByAgentAsync(string agentId, CancellationToken cancellationToken = default);

        // Métodos de mantenimiento requeridos por el controlador de agentes
        Task<Result> CheckCatalogsAsync(CancellationToken cancellationToken = default);
        Task PopulateFormAsync(SavePropertyViewModel vm, string agentId, CancellationToken cancellationToken = default);
        Task<Result<int>> CreateAsync(SavePropertyViewModel vm, IReadOnlyList<ImageUploadDto> images, string agentId, CancellationToken cancellationToken = default);
        Task<Result<SavePropertyViewModel>> GetForEditAsync(int id, string agentId, CancellationToken cancellationToken = default);
        Task<Result> UpdateAsync(SavePropertyViewModel vm, IReadOnlyList<ImageUploadDto> newImages, string agentId, CancellationToken cancellationToken = default);
        Task<Result<PropertyListViewModel>> GetForDeleteAsync(int id, string agentId, CancellationToken cancellationToken = default);
        Task<Result> DeleteAsync(int id, string agentId, CancellationToken cancellationToken = default);
    }
}