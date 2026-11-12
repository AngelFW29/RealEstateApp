using RealEstateApp.Core.Application.Dtos;
using RealEstateApp.Core.Application.Dtos.User;

namespace RealEstateApp.Core.Application.Interfaces
{
    public interface IAccountService
    {
        Task<List<UserSummaryDto>> GetUsersByRoleAsync(string role, CancellationToken cancellationToken = default);
        Task<UserSummaryDto?> GetUserWithRoleAsync(string userId, string role, CancellationToken cancellationToken = default);
        Task<Result> CreateUserAsync(RegisterUserDto dto, string role, CancellationToken cancellationToken = default);
        Task<Result> UpdateUserAsync(UpdateUserDto dto, string role, CancellationToken cancellationToken = default);
        Task<Result> SetActiveAsync(string userId, bool isActive, CancellationToken cancellationToken = default);
        Task<bool> EmailExistsAsync(string email, string? excludeUserId = null, CancellationToken cancellationToken = default);
        Task<bool> UserNameExistsAsync(string userName, string? excludeUserId = null, CancellationToken cancellationToken = default);
        Task<bool> CedulaExistsAsync(string cedula, string? excludeUserId = null, CancellationToken cancellationToken = default);
        Task<int> CountActiveByRoleAsync(string role, CancellationToken cancellationToken = default);
    }
}
