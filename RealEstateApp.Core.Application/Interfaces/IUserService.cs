using RealEstateApp.Core.Application.Dtos.User;

namespace RealEstateApp.Core.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserContactDto?> GetBasicInfoByIdAsync(string id);
        Task<Dictionary<string, UserContactDto>> GetBasicInfoByIdsAsync(IEnumerable<string> ids);
        Task<List<AgentBasicDto>> GetActiveAgentsAsync(string? searchTerm = null);
        Task<List<string>> GetActiveAgentIdsAsync();

        // Additional methods for user management
        Task<List<UserSummaryDto>> GetUsersByRoleAsync(string role, CancellationToken cancellationToken = default);
        Task<UserSummaryDto?> GetUserWithRoleAsync(string userId, string role, CancellationToken cancellationToken = default);
        Task<Result> SetActiveAsync(string userId, bool isActive, CancellationToken cancellationToken = default);
        Task<Result> DeleteUserAsync(string userId, CancellationToken cancellationToken = default);
    }
}