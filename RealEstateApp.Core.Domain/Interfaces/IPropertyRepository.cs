using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Core.Domain.Interfaces
{
    public interface IPropertyRepository : IGenericRepository<Property>
    {
        Task<List<string>> GetImagePathsByPropertyTypeIdAsync(int propertyTypeId, CancellationToken cancellationToken = default);
        Task<List<string>> GetImagePathsBySaleTypeIdAsync(int saleTypeId, CancellationToken cancellationToken = default);
        Task<List<Property>> GetAvailableByAgentAsync(string agentId, CancellationToken cancellationToken = default);
        Task<Property?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default);
        Task<Property> CreateWithDetailsAsync(Property property, IEnumerable<int> improvementIds, IEnumerable<PropertyImage> images, CancellationToken cancellationToken = default);
        Task UpdateWithDetailsAsync(Property property, IEnumerable<int> improvementIds, IEnumerable<int> imageIdsToRemove, IEnumerable<PropertyImage> newImages, CancellationToken cancellationToken = default);
        Task DeleteAsync(Property property, CancellationToken cancellationToken = default);

        // Additional methods for agent-specific operations
        Task<Dictionary<string, int>> CountByAgentIdsAsync(IEnumerable<string> agentIds, CancellationToken cancellationToken = default);
        Task<List<string>> GetImagePathsByAgentAsync(string agentId, CancellationToken cancellationToken = default);
        Task DeleteByAgentAsync(string agentId, CancellationToken cancellationToken = default);
    }
}