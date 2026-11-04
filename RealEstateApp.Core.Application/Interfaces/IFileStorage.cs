using Microsoft.AspNetCore.Http;

namespace RealEstateApp.Core.Application.Interfaces
{
    public interface IFileStorage
    {
        Task<string> SaveAsync(IFormFile file, string folder);
        void Delete(string? relativePath);
        void DeleteFolder(string folder);
    }
}