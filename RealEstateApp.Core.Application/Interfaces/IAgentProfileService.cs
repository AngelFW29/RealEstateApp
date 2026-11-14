using RealEstateApp.Core.Application.Dtos.Property;
using RealEstateApp.Core.Application.ViewModels.Agent;

namespace RealEstateApp.Core.Application.Interfaces
{
    public interface IAgentProfileService
    {
        Task<Result<AgentProfileViewModel>> GetAsync(string userId, CancellationToken cancellationToken = default);
        Task<Result> UpdateAsync(string userId, AgentProfileViewModel vm, ImageUploadDto? newPhoto, CancellationToken cancellationToken = default);
    }
}
