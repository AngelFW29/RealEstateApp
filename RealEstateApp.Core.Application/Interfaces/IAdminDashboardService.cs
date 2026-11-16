using RealEstateApp.Core.Application.ViewModels.Admin;

namespace RealEstateApp.Core.Application.Interfaces
{
    public interface IAdminDashboardService
    {
        Task<AdminDashboardViewModel> GetAsync(CancellationToken cancellationToken = default);
    }
}
