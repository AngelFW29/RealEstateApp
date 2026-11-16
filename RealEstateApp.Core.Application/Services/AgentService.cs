using RealEstateApp.Core.Application.Interfaces;

namespace RealEstateApp.Core.Application.Services
{
    public class AgentService : StaffUserService, IAgentService
    {
        public AgentService(IAccountService accountService)
            : base(accountService, "Agente", "agente")
        {
        }

        protected override async Task<Result> ValidateEditAsync(string id, string currentUserId, CancellationToken cancellationToken)
        {
            var user = await _accountService.GetUserWithRoleAsync(id, _roleName, cancellationToken);
            if (user == null)
            {
                return Result.Fail("El agente seleccionado no existe.");
            }
            return Result.Ok();
        }

        protected override async Task<Result> ValidateStatusChangeAsync(string id, bool isActive, string currentUserId, CancellationToken cancellationToken)
        {
            var user = await _accountService.GetUserWithRoleAsync(id, _roleName, cancellationToken);
            if (user == null)
            {
                return Result.Fail("El agente seleccionado no existe.");
            }
            return Result.Ok();
        }
    }
}
