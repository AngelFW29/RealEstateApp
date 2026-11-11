using AutoMapper;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Admin;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Services
{
    public class AgentManagementService : IAgentManagementService
    {
        private readonly IUserService _userService;
        private readonly IPropertyRepository _propertyRepository;
        private readonly IFileStorage _fileService;
        private readonly IMapper _mapper;

        public AgentManagementService(
            IUserService userService,
            IPropertyRepository propertyRepository,
            IFileStorage fileService,
            IMapper mapper)
        {
            _userService = userService;
            _propertyRepository = propertyRepository;
            _fileService = fileService;
            _mapper = mapper;
        }

        public async Task<List<AdminAgentListViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var users = await _userService.GetUsersByRoleAsync("Agente", cancellationToken);
            if (users == null || !users.Any()) return [];

            var agentIds = users.Select(u => u.Id).ToList();
            var counts = await _propertyRepository.CountByAgentIdsAsync(agentIds, cancellationToken);

            var vms = _mapper.Map<List<AdminAgentListViewModel>>(users);
            foreach (var vm in vms)
            {
                vm.PropertyCount = counts.TryGetValue(vm.Id, out int count) ? count : 0;
            }

            return vms;
        }

        public async Task<ConfirmAgentActionViewModel?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var user = await _userService.GetUserWithRoleAsync(id, "Agente", cancellationToken);
            if (user == null) return null;

            return new ConfirmAgentActionViewModel
            {
                Id = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                IsActive = user.IsActive
            };
        }

        public async Task<Result> SetActiveAsync(string id, bool isActive, CancellationToken cancellationToken = default)
        {
            var user = await _userService.GetUserWithRoleAsync(id, "Agente", cancellationToken);
            if (user == null)
            {
                return Result.Fail("El agente seleccionado no existe.");
            }

            return await _userService.SetActiveAsync(id, isActive, cancellationToken);
        }

        public async Task<Result> DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var user = await _userService.GetUserWithRoleAsync(id, "Agente", cancellationToken);
            if (user == null)
            {
                return Result.Fail("El agente seleccionado no existe.");
            }

            // 1. Obtener rutas de imágenes físicas asociadas a las propiedades del agente
            var imagePaths = await _propertyRepository.GetImagePathsByAgentAsync(id, cancellationToken);

            try
            {
                // 2. Eliminar todas las propiedades, mejoras, ofertas, favoritos y chats del agente en BD de forma atómica
                await _propertyRepository.DeleteByAgentAsync(id, cancellationToken);

                // 3. Eliminar el usuario de Identity (y sus roles/claims)
                var deleteResult = await _userService.DeleteUserAsync(id, cancellationToken);
                if (!deleteResult.Success)
                {
                    return Result.Fail("No fue posible eliminar el agente. Intente nuevamente más tarde.");
                }

                // 4. SOLO si Identity confirmó el éxito, borrar los archivos físicos
                foreach (var path in imagePaths)
                {
                    _fileService.Delete(path);
                }

                return Result.Ok();
            }
            catch (Exception)
            {
                return Result.Fail("No fue posible eliminar el agente. Intente nuevamente más tarde.");
            }
        }
    }
}
