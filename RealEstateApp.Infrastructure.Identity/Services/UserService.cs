using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application;
using RealEstateApp.Core.Application.Dtos.User;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Infrastructure.Identity.Entities;

namespace RealEstateApp.Infrastructure.Identity.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;

        public UserService(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<UserContactDto?> GetBasicInfoByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;
            var user = await _userManager.FindByIdAsync(id);
            return user == null ? null : Map(user);
        }

        public async Task<Dictionary<string, UserContactDto>> GetBasicInfoByIdsAsync(IEnumerable<string> ids)
        {
            var idList = ids?
                .Where(i => !string.IsNullOrWhiteSpace(i))
                .Distinct()
                .ToList() ?? new List<string>();

            if (idList.Count == 0) return new Dictionary<string, UserContactDto>();

            var users = await _userManager.Users
                .Where(u => idList.Contains(u.Id))
                .AsNoTracking()
                .ToListAsync();

            return users.ToDictionary(u => u.Id, Map);
        }

        public async Task<List<AgentBasicDto>> GetActiveAgentsAsync(string? searchTerm = null)
        {
            var agents = await _userManager.GetUsersInRoleAsync(Roles.Agent);
            IEnumerable<AppUser> query = agents.Where(a => a.IsActive);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim();
                query = query.Where(a =>
                    (a.FirstName?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (a.LastName?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false));
            }

            return query
                .OrderBy(a => a.FirstName)
                .ThenBy(a => a.LastName)
                .Select(a => new AgentBasicDto
                {
                    Id = a.Id,
                    FullName = BuildFullName(a),
                    PhotoPath = a.PhotoPath
                })
                .ToList();
        }

        public async Task<List<string>> GetActiveAgentIdsAsync()
        {
            var agents = await _userManager.GetUsersInRoleAsync(Roles.Agent);
            return agents.Where(a => a.IsActive).Select(a => a.Id).ToList();
        }

        private static UserContactDto Map(AppUser user) => new()
        {
            Id = user.Id,
            FullName = BuildFullName(user),
            Phone = user.PhoneNumber ?? string.Empty,
            Email = user.Email ?? string.Empty,
            PhotoPath = user.PhotoPath
        };

        private static string BuildFullName(AppUser user) =>
            $"{user.FirstName} {user.LastName}".Trim();

        public async Task<List<UserSummaryDto>> GetUsersByRoleAsync(string role, CancellationToken cancellationToken = default)
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(role);
            return usersInRole.Select(u => new UserSummaryDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email ?? string.Empty,
                IsActive = u.IsActive
            }).ToList();
        }
        public async Task<UserSummaryDto?> GetUserWithRoleAsync(string userId, string role, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            bool isInRole = await _userManager.IsInRoleAsync(user, role);
            if (!isInRole) return null;

            return new UserSummaryDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                IsActive = user.IsActive
            };
        }
        public async Task<Result> SetActiveAsync(string userId, bool isActive, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result.Fail("El usuario seleccionado no existe.");
            }

            user.IsActive = isActive;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return Result.Fail("No fue posible actualizar el estado del usuario.");
            }

            if (!isActive)
            {
                // Invalidar sesiones abiertas inmediatamente al inactivar
                await _userManager.UpdateSecurityStampAsync(user);
            }

            return Result.Ok();
        }
        public async Task<Result> DeleteUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result.Fail("El usuario seleccionado no existe.");
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return Result.Fail("No fue posible eliminar el usuario.");
            }

            return Result.Ok();
        }

    }
}