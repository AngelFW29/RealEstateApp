using Microsoft.AspNetCore.Http;

namespace RealEstateApp.Core.Application
{
    public static class ImageHelper
    {
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png" };

        public static bool IsValid(IFormFile? file)
        {
            if (file == null || file.Length == 0) return false;
            if (string.IsNullOrEmpty(file.FileName)) return false;

            var extension = Path.GetExtension(file.FileName);
            if (string.IsNullOrEmpty(extension)) return false;
            if (!AllowedExtensions.Contains(extension.ToLowerInvariant())) return false;

            try
            {
                using var stream = file.OpenReadStream();
                var header = new byte[4];
                if (stream.Read(header, 0, 4) < 4) return false;

                bool isJpg = header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF;
                bool isPng = header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47;

                return isJpg || isPng;
            }
            catch
            {
                return false;
            }
        }
    }
}