using AutoMapper;
using RealEstateApp.Core.Application.Dtos.Favorite;
using RealEstateApp.Core.Application.Dtos.Property;
using RealEstateApp.Core.Application.Exceptions;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Services
{
    public class FavoriteService : GenericService<FavoriteDto, Favorite>, IFavoriteService
    {
        private readonly IPropertyService _propertyService;
        public FavoriteService(IGenericRepository<Favorite> repository, IPropertyService propertyService,
            IMapper mapper) : base(repository, mapper)
        {
            _propertyService = propertyService;
        }
        public async Task<bool> ToggleAsync(string clientId, int propertyId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(clientId);

            var existing = (await _repository.GetAllAsync(
                f => f.ClientId == clientId && f.PropertyId == propertyId)).FirstOrDefault();

            if (existing is not null)
            {
                await _repository.DeleteAsync(existing);
                return false;
            }

            var property = await _propertyService.GetAvailableByIdAsync(propertyId)
                ?? throw new BusinessRuleException("La propiedad solicitada no existe o no se encuentra disponible.");

            await _repository.AddAsync(new Favorite { Id = 0, ClientId = clientId, PropertyId = property.Id });
            return true;
        }
        public async Task<List<int>> GetFavoritePropertyIdsAsync(string clientId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(clientId);

            var favorites = await _repository.GetAllAsync(f => f.ClientId == clientId);
            return favorites.Select(f => f.PropertyId).ToList();
        }
        public async Task<List<PropertyDto>> GetFavoritePropertiesAsync(string clientId, PropertyFilterDto filter)
        {
            var favoriteIds = (await GetFavoritePropertyIdsAsync(clientId)).ToHashSet();

            if (favoriteIds.Count == 0)
                return [];

            var available = await _propertyService.GetAvailableAsync(filter);
            return available.Where(p => favoriteIds.Contains(p.Id)).ToList();
        }
    }
}