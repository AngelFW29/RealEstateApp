using AutoMapper;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Services
{
    public class PropertyTypeService : CatalogService<PropertyType>, IPropertyTypeService
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IFileStorage _fileStorage;

        protected override string SingularNameMale => "tipo de propiedad";
        protected override string SingularNameFemale => string.Empty;
        protected override bool IsFemale => false;

        public PropertyTypeService(
            IPropertyTypeRepository repository,
            IPropertyRepository propertyRepository,
            IFileStorage fileStorage,
            IMapper mapper) : base(repository, mapper)
        {
            _propertyRepository = propertyRepository;
            _fileStorage = fileStorage;
        }

        protected override async Task PerformDeleteAsync(PropertyType entity, CancellationToken cancellationToken)
        {
            var imagePaths = await _propertyRepository.GetImagePathsByPropertyTypeIdAsync(entity.Id, cancellationToken);

            await _repository.DeleteAsync(entity);

            foreach (var path in imagePaths)
            {
                _fileStorage.Delete(path);
            }
        }
    }
}
