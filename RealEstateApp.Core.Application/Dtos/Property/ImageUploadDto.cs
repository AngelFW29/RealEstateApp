using System;
using System.Collections.Generic;
using System.Text;

namespace RealEstateApp.Core.Application.Dtos.Property
{
    public class ImageUploadDto
    {
        public required string FileName { get; set; }
        public required string ContentType { get; set; }
        public long Length { get; set; }
        public required Stream Content { get; set; }
    }
}
