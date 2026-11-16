using AutoMapper;
using RealEstateApp.Core.Application.Dtos.Property;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Property;
using RealEstateApp.Core.Domain.Common.Enums;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Services
{
    public partial class PropertyService : GenericService<PropertyDto, Property>, IPropertyService
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly ISaleTypeRepository _saleTypeRepository;
        private readonly IImprovementRepository _improvementRepository;
        private readonly IFileStorage _iFileStorage;
        private readonly IPropertyTypeRepository _propertyTypeRepository;
        private readonly IUserService _userService;

        public PropertyService(
            IImprovementRepository improvementRepository,
            IFileStorage iFileStorage,
            ISaleTypeRepository saleTypeRepository,
            IPropertyRepository repository,
            IPropertyTypeRepository propertyTypeRepository,
            IUserService userService,
            IMapper mapper) : base(repository, mapper)
        {
            _propertyTypeRepository = propertyTypeRepository;
            _userService = userService;
            _iFileStorage = iFileStorage;
            _saleTypeRepository = saleTypeRepository;
            _improvementRepository = improvementRepository;
            _propertyRepository = repository;
        }
        public async Task<List<PropertyListViewModel>> GetAllByAgentAsync(string agentId, CancellationToken cancellationToken = default)
        {
            var properties = await _propertyRepository.GetAllByAgentIdAsync(agentId, cancellationToken);

            return properties.Select(p => new PropertyListViewModel
            {
                Id = p.Id,
                Code = p.Code,
                PropertyTypeName = p.PropertyType?.Name ?? string.Empty,
                SaleTypeName = p.SaleType?.Name ?? string.Empty,
                MainImageUrl = p.Images?.FirstOrDefault()?.ImagePath ?? string.Empty,
                Price = p.Price,
                Bedrooms = p.Bedrooms,
                Bathrooms = p.Bathrooms,
                Size = ((double)p.SizeInMeters),
                IsSold = p.Status == PropertyStatus.Sold
            }).ToList();
        }
    }

}