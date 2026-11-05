using RealEstateApp.Core.Domain.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace RealEstateApp.Core.Application.Dtos.Property
{
    public class PropertyDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal SizeInMeters { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public PropertyStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public int PropertyTypeId { get; set; }
        public string PropertyTypeName { get; set; } = string.Empty;

        public int SaleTypeId { get; set; }
        public string SaleTypeName { get; set; } = string.Empty;

        public string? MainImagePath { get; set; }
        public List<string> ImagePaths { get; set; } = [];
        public List<string> ImprovementNames { get; set; } = [];

        public string AgentId { get; set; } = string.Empty;
        public string AgentName { get; set; } = string.Empty;
        public string? AgentPhone { get; set; }
        public string? AgentEmail { get; set; }
        public string? AgentPhotoPath { get; set; }
    }
}