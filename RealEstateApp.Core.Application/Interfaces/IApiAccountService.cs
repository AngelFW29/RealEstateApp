using System.Threading.Tasks;
using RealEstateApp.Core.Application.Dtos; 

namespace RealEstateApp.Core.Application.Interfaces
{
    public interface IApiAccountService
    {
        Task<AuthResult> AuthenticateAsync(LoginDto dto);
        Task<Result> RegisterAsync(RegisterUserDto dto, string role);
    }
}