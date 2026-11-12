using RealEstateApp.Core.Application.Interfaces;

namespace RealEstateApp.Core.Application.Services
{
    public class DeveloperService : StaffUserService, IDeveloperService
    {
        public DeveloperService(IAccountService accountService)
            : base(accountService, "Desarrollador", "desarrollador")
        {
        }

        protected override async Task<Result> ValidateEditAsync(string id, string currentUserId, CancellationToken cancellationToken)
        {
            var user = await _accountService.GetUserWithRoleAsync(id, _roleName, cancellationToken);
            if (user == null)
            {
                return Result.Fail("El desarrollador seleccionado no existe.");
            }
            return Result.Ok();
        }

        protected override async Task<Result> ValidateStatusChangeAsync(string id, bool isActive, string currentUserId, CancellationToken cancellationToken)
        {
            var user = await _accountService.GetUserWithRoleAsync(id, _roleName, cancellationToken);
            if (user == null)
            {
                return Result.Fail("El desarrollador seleccionado no existe.");
            }
            return Result.Ok();
        }
    }
}
