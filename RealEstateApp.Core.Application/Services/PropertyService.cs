using AutoMapper;
using RealEstateApp.Core.Application.Dtos.Property;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Services
{
    public partial class PropertyService : GenericService<PropertyDto, Property>, IPropertyService
    {
        private readonly IPropertyTypeRepository _propertyTypeRepository;
        private readonly IUserService _userService;

        public PropertyService(
            IPropertyRepository repository,
            IPropertyTypeRepository propertyTypeRepository, 
            IUserService userService,
            IMapper mapper) : base(repository, mapper)
        {
            _propertyTypeRepository = propertyTypeRepository;
            _userService = userService;
        }
    }
}