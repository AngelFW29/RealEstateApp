using RealEstateApp.Core.Application.ViewModels.Admin;

namespace RealEstateApp.Core.Application.Interfaces
{
    public interface IAgentManagementService
    {
        Task<List<AdminAgentListViewModel>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ConfirmAgentActionViewModel?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<Result> SetActiveAsync(string id, bool isActive, CancellationToken cancellationToken = default);
        Task<Result> DeleteAsync(string id, CancellationToken cancellationToken = default);
    }
}
