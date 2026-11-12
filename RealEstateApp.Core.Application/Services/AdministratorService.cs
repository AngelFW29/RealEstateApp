using RealEstateApp.Core.Application.Interfaces;

namespace RealEstateApp.Core.Application.Services
{
    public class AdministratorService : StaffUserService, IAdministratorService
    {
        public AdministratorService(IAccountService accountService)
            : base(accountService, "Administrador", "administrador")
        {
        }

        protected override async Task<Result> ValidateEditAsync(string id, string currentUserId, CancellationToken cancellationToken)
        {
            if (id == currentUserId)
            {
                return Result.Fail("No puede editar su propio usuario desde este mantenimiento.");
            }

            var user = await _accountService.GetUserWithRoleAsync(id, _roleName, cancellationToken);
            if (user == null)
            {
                return Result.Fail("El administrador seleccionado no existe.");
            }

            return Result.Ok();
        }

        protected override async Task<Result> ValidateStatusChangeAsync(string id, bool isActive, string currentUserId, CancellationToken cancellationToken)
        {
            if (id == currentUserId && !isActive)
            {
                return Result.Fail("No puede inactivar a su propio usuario.");
            }

            var user = await _accountService.GetUserWithRoleAsync(id, _roleName, cancellationToken);
            if (user == null)
            {
                return Result.Fail("El administrador seleccionado no existe.");
            }

            if (!isActive)
            {
                int activeCount = await _accountService.CountActiveByRoleAsync(_roleName, cancellationToken);
                if (activeCount <= 1 && user.IsActive)
                {
                    return Result.Fail("Debe existir al menos un administrador activo en el sistema.");
                }
            }

            return Result.Ok();
        }
    }
}
