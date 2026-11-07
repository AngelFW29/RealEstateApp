using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using RealEstateApp.Core.Application.Interfaces;

namespace RealEstateApp.Infrastructure.Shared.Services
{
    public class LocalFileStorage : IFileStorage
    {
        private readonly string _webRoot;

        public LocalFileStorage(IWebHostEnvironment env, IConfiguration config)
        {
            var configuredPath = config["Storage:WebRootPath"] ?? "wwwroot";
            _webRoot = Path.GetFullPath(Path.Combine(env.ContentRootPath, configuredPath));
        }
        
        public async Task<string> SaveAsync(Stream stream, string fileName, string folder, CancellationToken cancellationToken = default)
        {
            var dir = Path.Combine(_webRoot, "uploads", folder.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(dir);

            var name = $"{Guid.NewGuid():N}{Path.GetExtension(fileName).ToLowerInvariant()}";
            var fullPath = Path.Combine(dir, name);

            await using var fs = new FileStream(fullPath, FileMode.Create);
            await stream.CopyToAsync(fs, cancellationToken);

            return $"/uploads/{folder}/{name}".Replace("\\", "/");
        }
        
        public async Task<string> SaveAsync(IFormFile file, string folder)
        {
            var dir = Path.Combine(_webRoot, "uploads", folder.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(dir);

            var name = $"{Guid.NewGuid():N}{Path.GetExtension(file.FileName).ToLowerInvariant()}";
            var fullPath = Path.Combine(dir, name);

            await using var fs = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(fs);

            return $"/uploads/{folder}/{name}";
        }

        public void Delete(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath) || relativePath.StartsWith("/img/")) return;

            var fullPath = Path.GetFullPath(Path.Combine(_webRoot, relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar)));

            // Seguridad: verificar que el archivo esté dentro del _webRoot antes de borrar
            if (fullPath.StartsWith(_webRoot) && File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        public void DeleteFolder(string folder)
        {
            var fullPath = Path.GetFullPath(Path.Combine(_webRoot, "uploads", folder.Replace('/', Path.DirectorySeparatorChar)));
            if (fullPath.StartsWith(_webRoot) && Directory.Exists(fullPath))
            {
                Directory.Delete(fullPath, true);
            }
        }
    }
}