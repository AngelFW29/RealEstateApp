using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application;
using RealEstateApp.Core.Application.Dtos;
using RealEstateApp.Core.Application.Dtos.User;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Infrastructure.Identity.Entities;

namespace RealEstateApp.Infrastructure.Identity.Services
{
    public class UserService : IUserService, IAccountService
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
                Cedula = user.IdCard,
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
        #region IAccountService Members
        public async Task<Result> CreateUserAsync(RegisterUserDto dto, string role, CancellationToken cancellationToken = default)
        {
            var user = new AppUser
            {
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                IdCard = string.IsNullOrWhiteSpace(dto.IdCard) ? null : NormalizeCedula(dto.IdCard),
                Email = dto.Email.Trim(),
                UserName = dto.UserName.Trim(),
                IsActive = true,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                return Result.Fail(result.Errors.Select(e => TranslateIdentityError(e)).ToArray());
            }

            await _userManager.AddToRoleAsync(user, role);
            return Result.Ok();
        }

        public async Task<Result> UpdateUserAsync(UpdateUserDto dto, string role, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(dto.Id);
            if (user == null || !await _userManager.IsInRoleAsync(user, role))
            {
                return Result.Fail("El usuario seleccionado no existe.");
            }

            user.FirstName = dto.FirstName.Trim();
            user.LastName = dto.LastName.Trim();
            user.IdCard = string.IsNullOrWhiteSpace(dto.Cedula) ? null : NormalizeCedula(dto.Cedula);

            await _userManager.SetEmailAsync(user, dto.Email.Trim());
            await _userManager.SetUserNameAsync(user, dto.UserName.Trim());

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return Result.Fail(updateResult.Errors.Select(e => TranslateIdentityError(e)).ToArray());
            }

            if (!string.IsNullOrEmpty(dto.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passResult = await _userManager.ResetPasswordAsync(user, token, dto.Password);
                if (!passResult.Succeeded)
                {
                    return Result.Fail(passResult.Errors.Select(e => TranslateIdentityError(e)).ToArray());
                }
                await _userManager.UpdateSecurityStampAsync(user);
            }

            return Result.Ok();
        }

        public async Task<bool> EmailExistsAsync(string email, string? excludeUserId = null, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email.Trim());
            if (user == null) return false;
            return user.Id != excludeUserId;
        }

        public async Task<bool> UserNameExistsAsync(string userName, string? excludeUserId = null, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByNameAsync(userName.Trim());
            if (user == null) return false;
            return user.Id != excludeUserId;
        }

        public async Task<bool> CedulaExistsAsync(string cedula, string? excludeUserId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(cedula)) return false;
            string norm = NormalizeCedula(cedula);

            var users = await _userManager.Users.Where(u => u.IdCard != null).ToListAsync(cancellationToken);
            return users.Any(u => NormalizeCedula(u.IdCard!) == norm && u.Id != excludeUserId);
        }

        public async Task<int> CountActiveByRoleAsync(string role, CancellationToken cancellationToken = default)
        {
            var users = await _userManager.GetUsersInRoleAsync(role);
            return users.Count(u => u.IsActive);
        }

        private string NormalizeCedula(string cedula)
        {
            return new string(cedula.Where(char.IsDigit).ToArray());
        }

        private string TranslateIdentityError(IdentityError error)
        {
            return error.Code switch
            {
                "DuplicateEmail" => "Ya existe un usuario registrado con este correo electrónico.",
                "DuplicateUserName" => "Ya existe un usuario registrado con este nombre de usuario.",
                "PasswordTooShort" => "La contraseña debe ser más larga.",
                _ => error.Description
            };
        }
        #endregion

    }
}