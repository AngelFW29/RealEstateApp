using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Staff;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Services
{
    public class AgentService : StaffUserService, IAgentService
    {
        private readonly IPropertyRepository _propertyRepository;
        public AgentService(IAccountService accountService, IPropertyRepository propertyRepository)
            : base(accountService, "Agente", "agente")
        {
            _propertyRepository = propertyRepository;
        }
        public override async Task<List<StaffUserListViewModel>> GetAllAsync(string currentUserId, CancellationToken cancellationToken = default)
        {
            var agents = await base.GetAllAsync(currentUserId, cancellationToken);

            if (agents.Count == 0) return agents;

            var counts = await _propertyRepository.CountByAgentIdsAsync(agents.Select(a => a.Id), cancellationToken);

            foreach (var agent in agents)
            {
                agent.PropertyCount = counts.TryGetValue(agent.Id, out int total) ? total : 0;
            }

            return agents;
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
