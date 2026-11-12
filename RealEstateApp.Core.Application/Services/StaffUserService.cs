using RealEstateApp.Core.Application.Dtos.User;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Staff;

namespace RealEstateApp.Core.Application.Services
{
    public abstract class StaffUserService : IStaffUserService
    {
        protected readonly IAccountService _accountService;
        protected readonly string _roleName;
        protected readonly string _entityName;

        protected StaffUserService(IAccountService accountService, string roleName, string entityName)
        {
            _accountService = accountService;
            _roleName = roleName;
            _entityName = entityName;
        }

        public virtual async Task<List<StaffUserListViewModel>> GetAllAsync(string currentUserId, CancellationToken cancellationToken = default)
        {
            var users = await _accountService.GetUsersByRoleAsync(_roleName, cancellationToken);
            return users.Select(u => new StaffUserListViewModel
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                UserName = u.UserName,
                Cedula = u.Cedula,
                Email = u.Email,
                IsActive = u.IsActive,
                IsCurrentUser = u.Id == currentUserId
            }).ToList();
        }

        public virtual async Task<EditStaffUserViewModel?> GetForEditAsync(string id, string currentUserId, CancellationToken cancellationToken = default)
        {
            var user = await _accountService.GetUserWithRoleAsync(id, _roleName, cancellationToken);
            if (user == null) return null;

            return new EditStaffUserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Cedula = user.Cedula ?? string.Empty,
                Email = user.Email,
                UserName = user.UserName,
                IsActive = user.IsActive
            };
        }

        public virtual async Task<Result> CreateAsync(CreateStaffUserViewModel vm, CancellationToken cancellationToken = default)
        {
            if (await _accountService.EmailExistsAsync(vm.Email, null, cancellationToken))
                return Result.Fail("Ya existe un usuario registrado con este correo electrónico.");

            if (await _accountService.UserNameExistsAsync(vm.UserName, null, cancellationToken))
                return Result.Fail("Ya existe un usuario registrado con este nombre de usuario.");

            if (!string.IsNullOrWhiteSpace(vm.Cedula) && await _accountService.CedulaExistsAsync(vm.Cedula, null, cancellationToken))
                return Result.Fail("Ya existe un usuario registrado con esta cédula.");

            var dto = new Dtos.RegisterUserDto
            {
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                IdCard = vm.Cedula,
                Email = vm.Email,
                UserName = vm.UserName,
                Password = vm.Password
            };

            var result = await _accountService.CreateUserAsync(dto, _roleName, cancellationToken);
            if (!result.Success) return result;

            return Result.Ok($"El {_entityName} fue creado correctamente.");
        }

        public virtual async Task<Result> UpdateAsync(EditStaffUserViewModel vm, string currentUserId, CancellationToken cancellationToken = default)
        {
            var customValidation = await ValidateEditAsync(vm.Id, currentUserId, cancellationToken);
            if (!customValidation.Success) return customValidation;

            if (await _accountService.EmailExistsAsync(vm.Email, vm.Id, cancellationToken))
                return Result.Fail("Ya existe un usuario registrado con este correo electrónico.");

            if (await _accountService.UserNameExistsAsync(vm.UserName, vm.Id, cancellationToken))
                return Result.Fail("Ya existe un usuario registrado con este nombre de usuario.");

            if (!string.IsNullOrWhiteSpace(vm.Cedula) && await _accountService.CedulaExistsAsync(vm.Cedula, vm.Id, cancellationToken))
                return Result.Fail("Ya existe un usuario registrado con esta cédula.");

            var dto = new UpdateUserDto
            {
                Id = vm.Id,
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                Cedula = vm.Cedula,
                Email = vm.Email,
                UserName = vm.UserName,
                Password = vm.NewPassword
            };

            var result = await _accountService.UpdateUserAsync(dto, _roleName, cancellationToken);
            if (!result.Success) return result;

            return Result.Ok($"El {_entityName} fue actualizado correctamente.");
        }

        public virtual async Task<Result> SetActiveAsync(string id, bool isActive, string currentUserId, CancellationToken cancellationToken = default)
        {
            var customValidation = await ValidateStatusChangeAsync(id, isActive, currentUserId, cancellationToken);
            if (!customValidation.Success) return customValidation;

            var result = await _accountService.SetActiveAsync(id, isActive, cancellationToken);
            if (!result.Success) return result;

            string actionText = isActive ? "activado" : "inactivado";
            return Result.Ok($"El {_entityName} fue {actionText} correctamente.");
        }

        protected virtual Task<Result> ValidateEditAsync(string id, string currentUserId, CancellationToken cancellationToken)
        {
            return Task.FromResult(Result.Ok());
        }

        protected virtual Task<Result> ValidateStatusChangeAsync(string id, bool isActive, string currentUserId, CancellationToken cancellationToken)
        {
            return Task.FromResult(Result.Ok());
        }
    }
}
