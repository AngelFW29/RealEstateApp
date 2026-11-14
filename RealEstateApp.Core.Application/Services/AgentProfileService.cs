using RealEstateApp.Core.Application.Dtos.Property;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Agent;

namespace RealEstateApp.Core.Application.Services
{
    public class AgentProfileService : IAgentProfileService
    {
        private readonly IAccountService _accountService;
        private readonly IFileStorage _fileStorage;
        private readonly IUserService _userService;

        public AgentProfileService(IAccountService accountService, IFileStorage fileStorage, IUserService userService)
        {
            _accountService = accountService;
            _fileStorage = fileStorage;
            _userService = userService;
        }

        public async Task<Result<AgentProfileViewModel>> GetAsync(string userId, CancellationToken cancellationToken = default)
        {
            var userDto = await _userService.GetBasicInfoByIdAsync(userId);
            if (userDto == null)
            {
                return Result<AgentProfileViewModel>.Fail("Usuario no encontrado.");
            }

            var vm = new AgentProfileViewModel
            {
                FullName = userDto.FullName,
                PhoneNumber = userDto.Phone ?? string.Empty,
                CurrentPhotoUrl = userDto.PhotoPath
            };

            return Result<AgentProfileViewModel>.Ok(vm);
        }

        public async Task<Result> UpdateAsync(string userId, AgentProfileViewModel vm, ImageUploadDto? newPhoto, CancellationToken cancellationToken = default)
        {
            string? newPhotoUrl = null;

            if (newPhoto != null)
            {
                if (!ImageHelper.IsValidimage(newPhoto))
                {
                    return Result.Fail("El archivo seleccionado no tiene un formato de imagen válido.");
                }

                newPhotoUrl = await _fileStorage.SaveAsync(newPhoto.Content, Path.GetExtension(newPhoto.FileName), "profiles");
            }

            var (updateResult, oldPhotoUrl) = await _accountService.UpdateUserProfileAsync(userId, vm.FullName, vm.PhoneNumber, newPhotoUrl);

            if (!updateResult.Success)
            {
                if (!string.IsNullOrEmpty(newPhotoUrl))
                {
                    _fileStorage.Delete(newPhotoUrl);
                }
                return Result.Fail(updateResult.Message ?? "Error al actualizar el perfil.");
            }

            if (!string.IsNullOrEmpty(newPhotoUrl) && !string.IsNullOrEmpty(oldPhotoUrl))
            {
                _fileStorage.Delete(oldPhotoUrl);
            }

            return Result.Ok();
        }
    }
}
