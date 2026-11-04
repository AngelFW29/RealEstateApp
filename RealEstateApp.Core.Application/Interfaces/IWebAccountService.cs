using System.Threading.Tasks;
using RealEstateApp.Core.Application.ViewModels;

namespace RealEstateApp.Core.Application.Interfaces
{
    public interface IWebAccountService
    {
        Task<Result<string>> LoginAsync(LoginViewModel vm);
        Task<Result> RegisterAsync(RegisterViewModel vm, string photoPath, string origin);
        Task<Result> ConfirmEmailAsync(string userId, string token);
        Task SignOutAsync();
    }
}