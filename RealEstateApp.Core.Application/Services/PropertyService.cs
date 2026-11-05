using AutoMapper;
using RealEstateApp.Core.Application.Dtos.Property;
using RealEstateApp.Core.Application.Exceptions;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Common.Enums;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;
using System.Linq.Expressions;

namespace RealEstateApp.Core.Application.Services
{
    public partial class PropertyService : GenericService<PropertyDto, Property>, IPropertyService
    {      
        private readonly IGenericRepository<PropertyType> _propertyTypeRepository;

        public PropertyService(IGenericRepository<Property> repository, IGenericRepository<PropertyType> propertyTypeRepository,
            IMapper mapper) : base(repository, mapper)
        {
            _propertyTypeRepository = propertyTypeRepository;
        }

        public async Task<List<PropertyDto>> GetAvailableAsync(PropertyFilterDto filter)
        {
            await EnsurePropertyTypeExistsAsync(filter.PropertyTypeId);

            var properties = await _repository.GetAllAsync(
                BuildFilter(filter, availableOnly: true, agentId: null), ListIncludes);

            return ToOrderedDtos(properties);
        }
        public async Task<PropertyDto?> GetAvailableByIdAsync(int id)
        {
            var property = await _repository.GetByIdAsync(id, DetailIncludes);

            return property is null || property.Status != PropertyStatus.Available
                ? null
                : _mapper.Map<PropertyDto>(property);
        }
        public async Task<List<PropertyDto>> GetByAgentAsync(string agentId, PropertyFilterDto filter)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(agentId);
            await EnsurePropertyTypeExistsAsync(filter.PropertyTypeId);

            var properties = await _repository.GetAllAsync(
                BuildFilter(filter, availableOnly: false, agentId: agentId), ListIncludes);

            return ToOrderedDtos(properties);
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
        #endregion
    }
}