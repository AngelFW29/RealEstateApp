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
    }
}