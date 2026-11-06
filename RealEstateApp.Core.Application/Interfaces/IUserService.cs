using RealEstateApp.Core.Application.Dtos.User;

namespace RealEstateApp.Core.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserContactDto?> GetBasicInfoByIdAsync(string id);
        Task<Dictionary<string, UserContactDto>> GetBasicInfoByIdsAsync(IEnumerable<string> ids);
        Task<List<AgentBasicDto>> GetActiveAgentsAsync(string? searchTerm = null);
        Task<List<string>> GetActiveAgentIdsAsync();
    }
}