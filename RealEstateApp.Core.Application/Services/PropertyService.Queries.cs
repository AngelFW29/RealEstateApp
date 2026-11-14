using RealEstateApp.Core.Application.Dtos.Property;
using RealEstateApp.Core.Application.Exceptions;
using RealEstateApp.Core.Domain.Common.Enums;
using RealEstateApp.Core.Domain.Entities;
using System.Linq.Expressions;

namespace RealEstateApp.Core.Application.Services
{
    public partial class PropertyService 
    {        
        public async Task<List<PropertyDto>> GetAvailableAsync(PropertyFilterDto filter)
        {
            await EnsurePropertyTypeExistsAsync(filter.PropertyTypeId);

            var properties = await _repository.GetAllAsync(
                BuildFilter(filter, availableOnly: true, agentId: null), ListIncludes);

            var visible = await KeepActiveAgentsAsync(properties);
            var dtos = ToOrderedDtos(visible);

            await FillAgentDataAsync(dtos);
            return dtos;
        }
        public async Task<PropertyDto?> GetAvailableByIdAsync(int id)
        {
            var property = await _repository.GetByIdAsync(id, DetailIncludes);

            if (property is null || property.Status != PropertyStatus.Available)
                return null;

            var visible = await KeepActiveAgentsAsync([property]);

            if (visible.Count == 0) return null;

            var dto = _mapper.Map<PropertyDto>(property);

            await FillAgentDataAsync([dto]);
            return dto;
        }
        public async Task<List<PropertyDto>> GetByAgentAsync(string agentId, PropertyFilterDto filter)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(agentId);
            await EnsurePropertyTypeExistsAsync(filter.PropertyTypeId);

            var properties = await _repository.GetAllAsync(
                BuildFilter(filter, availableOnly: false, agentId: agentId), ListIncludes);

            return ToOrderedDtos(properties);
        }
        public async Task<PropertyDto?> GetAgentPropertyByIdAsync(string agentId, int id)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(agentId);

            var property = await _repository.GetByIdAsync(id, DetailIncludes);

            return property is null || property.AgentId != agentId
                ? null
                : _mapper.Map<PropertyDto>(property);
        }
        public async Task<List<PropertyApiDto>> GetAllForApiAsync()
        {
            var properties = await _repository.GetAllAsync(null, ApiIncludes);

            var ordered = properties
                .OrderByDescending(p => p.CreatedAt)
                .ThenByDescending(p => p.Id);

            var dtos = _mapper.Map<List<PropertyApiDto>>(ordered);

            await FillAgentNamesAsync(dtos);
            return dtos;
        }
        public async Task<PropertyApiDto?> GetByIdForApiAsync(int id)
        {
            var property = await _repository.GetByIdAsync(id, ApiIncludes);
            return await ToApiDtoAsync(property);
        }
        public async Task<PropertyApiDto?> GetByCodeForApiAsync(string code)
        {
            var normalized = code.Trim();

            var property = (await _repository.GetAllAsync(p => p.Code == normalized, ApiIncludes)).FirstOrDefault();
            return await ToApiDtoAsync(property);
        }

        #region Private methods
        private async Task EnsurePropertyTypeExistsAsync(int? propertyTypeId)
        {
            if (propertyTypeId is null)
                return;

            var exists = await _propertyTypeRepository.CountAsync(t => t.Id == propertyTypeId) > 0;

            if (!exists)
                throw new BusinessRuleException("El tipo de propiedad seleccionado no existe.");
        }
        private async Task<List<Property>> KeepActiveAgentsAsync(List<Property> properties)
        {
            var activeAgentIds = (await _userService.GetActiveAgentIdsAsync()).ToHashSet();

            return properties.Where(p => activeAgentIds.Contains(p.AgentId)).ToList();
        }
        private async Task FillAgentDataAsync(List<PropertyDto> dtos)
        {
            if (dtos.Count == 0)
                return;

            var agents = await _userService.GetBasicInfoByIdsAsync(dtos.Select(d => d.AgentId).Distinct());

            foreach (var dto in dtos)
            {
                if (!agents.TryGetValue(dto.AgentId, out var agent))
                    continue;

                dto.AgentName = agent.FullName;
                dto.AgentPhone = agent.Phone;
                dto.AgentEmail = agent.Email;
                dto.AgentPhotoPath = agent.PhotoPath;
            }
        }
        private static Expression<Func<Property, bool>> BuildFilter(PropertyFilterDto filter, bool availableOnly, string? agentId)
        {
            var code = filter.Code?.Trim();
            var owner = agentId ?? filter.AgentId;

            return p =>
                (!availableOnly || p.Status == PropertyStatus.Available)
                && (string.IsNullOrEmpty(code) || p.Code == code)
                && (string.IsNullOrEmpty(owner) || p.AgentId == owner)
                && (filter.PropertyTypeId == null || p.PropertyTypeId == filter.PropertyTypeId)
                && (filter.MinPrice == null || p.Price >= filter.MinPrice)
                && (filter.MaxPrice == null || p.Price <= filter.MaxPrice)
                && (filter.Bedrooms == null || p.Bedrooms == filter.Bedrooms)
                && (filter.Bathrooms == null || p.Bathrooms == filter.Bathrooms);
        }
        private List<PropertyDto> ToOrderedDtos(IEnumerable<Property> properties)
        {
            var ordered = properties
                .OrderByDescending(p => p.CreatedAt)
                .ThenByDescending(p => p.Id);

            return _mapper.Map<List<PropertyDto>>(ordered);
        }
        private static readonly string[] ListIncludes = ["PropertyType", "SaleType", "Images"];
        private static readonly string[] DetailIncludes = ["PropertyType", "SaleType", "Images", "Improvements.Improvement"];
        
        //Api
        private static readonly string[] ApiIncludes = ["PropertyType", "SaleType", "Improvements.Improvement"];
        private async Task<PropertyApiDto?> ToApiDtoAsync(Property? property)
        {
            if (property is null)
                return null;

            var dto = _mapper.Map<PropertyApiDto>(property);

            await FillAgentNamesAsync([dto]);
            return dto;
        }
        private async Task FillAgentNamesAsync(List<PropertyApiDto> dtos)
        {
            if (dtos.Count == 0)
                return;

            var agents = await _userService.GetBasicInfoByIdsAsync(dtos.Select(d => d.AgentId).Distinct());

            foreach (var dto in dtos)
            {
                if (agents.TryGetValue(dto.AgentId, out var agent))
                    dto.AgentName = agent.FullName;
            }
        }
        #endregion
    }
}