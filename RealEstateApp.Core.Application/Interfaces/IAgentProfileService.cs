using Microsoft.AspNetCore.Http;
using RealEstateApp.Core.Application.ViewModels.Agent;

namespace RealEstateApp.Core.Application.Interfaces
{
    public interface IAgentProfileService
    {
        Task<Result<AgentProfileViewModel>> GetAsync(string userId, CancellationToken cancellationToken = default);
        Task<Result> UpdateAsync(string userId, AgentProfileViewModel vm, IFormFile? newPhoto, CancellationToken cancellationToken = default);
    }
}
