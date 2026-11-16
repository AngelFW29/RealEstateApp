using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Admin;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IAccountService _accountService;

        public AdminDashboardService(IPropertyRepository propertyRepository, IAccountService accountService)
        {
            _propertyRepository = propertyRepository;
            _accountService = accountService;
        }

        public async Task<AdminDashboardViewModel> GetAsync(CancellationToken cancellationToken = default)
        {
            var (available, sold) = await _propertyRepository.CountByStatusAsync();
            var roleStatusCounts = await _accountService.CountUsersByRoleAndStatusAsync();

            var vm = new AdminDashboardViewModel
            {
                AvailableProperties = available,
                SoldProperties = sold
            };

            foreach (var item in roleStatusCounts)
            {
                var role = item.Role;

                if (role == "Agente")
                {
                    if (item.IsActive) vm.ActiveAgents = item.Count;
                    else vm.InactiveAgents = item.Count;
                }
                else if (role == "Cliente")
                {
                    if (item.IsActive) vm.ActiveClients = item.Count;
                    else vm.InactiveClients = item.Count;
                }
                else if (role == "Desarrollador")
                {
                    if (item.IsActive) vm.ActiveDevelopers = item.Count;
                    else vm.InactiveDevelopers = item.Count;
                }
            }

            return vm;
        }
    }
}
