using RealEstateApp.Core.Application.Dtos.Agent;
using RealEstateApp.Core.Application.Dtos.Property;

namespace RealEstateApp.Core.Application.Interfaces
{
    public interface IAgentApiService
    {
        Task<List<AgentApiDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Result<AgentApiDto>> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<Result<List<PropertyDto>>> GetAgentPropertiesAsync(string id, CancellationToken cancellationToken = default);
        Task<Result> ChangeStatusAsync(string id, bool isActive, CancellationToken cancellationToken = default);
    }
}
