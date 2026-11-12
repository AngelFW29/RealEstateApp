using RealEstateApp.Core.Application.ViewModels.Staff;

namespace RealEstateApp.Core.Application.Interfaces
{
    public interface IStaffUserService
    {
        Task<List<StaffUserListViewModel>> GetAllAsync(string currentUserId, CancellationToken cancellationToken = default);
        Task<EditStaffUserViewModel?> GetForEditAsync(string id, string currentUserId, CancellationToken cancellationToken = default);
        Task<Result> CreateAsync(CreateStaffUserViewModel vm, CancellationToken cancellationToken = default);
        Task<Result> UpdateAsync(EditStaffUserViewModel vm, string currentUserId, CancellationToken cancellationToken = default);
        Task<Result> SetActiveAsync(string id, bool isActive, string currentUserId, CancellationToken cancellationToken = default);
    }

    public interface IAdministratorService : IStaffUserService { }
    public interface IDeveloperService : IStaffUserService { }
}
