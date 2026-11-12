using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using RealEstateApp.Core.Application.Interfaces;

namespace RealEstateApp.Infrastructure.Shared.Services
{
    public class LocalFileStorage : IFileStorage
    {
        private readonly string _root;

        public LocalFileStorage(IWebHostEnvironment env, IConfiguration config)
        {
            _root = Path.GetFullPath(
                Path.Combine(env.ContentRootPath, config["Storage:UploadsPath"] ?? "../Uploads"));
            Directory.CreateDirectory(_root);
        }

        public async Task<string> SaveAsync(IFormFile file, string folder)
        {
            var dir = Path.Combine(_root, folder.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(dir);

            var name = $"{Guid.NewGuid():N}{Path.GetExtension(file.FileName).ToLowerInvariant()}";
            await using var fs = new FileStream(Path.Combine(dir, name), FileMode.Create);
            await file.CopyToAsync(fs);

            return $"/uploads/{folder}/{name}";
        }

        public async Task<string> SaveAsync(Stream stream, string fileName, string folder, CancellationToken cancellationToken = default)
        {
            var dir = Path.Combine(_root, folder.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(dir);

            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            var name = $"{Guid.NewGuid():N}{extension}";
            var fullPath = Path.Combine(dir, name);

            await using var fs = new FileStream(fullPath, FileMode.Create);
            await stream.CopyToAsync(fs, cancellationToken);

            return $"/uploads/{folder}/{name}";
        }

        public void Delete(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath) || !relativePath.StartsWith("/uploads/")) return;

            var full = Path.GetFullPath(
                Path.Combine(_root, relativePath["/uploads/".Length..].Replace('/', Path.DirectorySeparatorChar)));

            if (full.StartsWith(_root) && File.Exists(full)) File.Delete(full);
        }

        public void DeleteFolder(string folder)
        {
            var full = Path.GetFullPath(
                Path.Combine(_root, folder.Replace('/', Path.DirectorySeparatorChar)));

            if (full.StartsWith(_root) && Directory.Exists(full)) Directory.Delete(full, true);
        }
    }
}