using RealEstateApp.Core.Application.Dtos.Property;

namespace RealEstateApp.Core.Application.Interfaces
{
    public interface IPropertyService : IGenericService<PropertyDto>
    {
        Task<List<PropertyDto>> GetAvailableAsync(PropertyFilterDto filter);
        Task<PropertyDto?> GetAvailableByIdAsync(int id);
        Task<List<PropertyDto>> GetByAgentAsync(string agentId, PropertyFilterDto filter);
        Task<PropertyDto?> GetAgentPropertyByIdAsync(string agentId, int id);
    }
}