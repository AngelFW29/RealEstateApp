using RealEstateApp.Core.Application.Dtos.Favorite;
using RealEstateApp.Core.Application.Dtos.Property;

namespace RealEstateApp.Core.Application.Interfaces
{
    public interface IFavoriteService : IGenericService<FavoriteDto>
    {
        Task<bool> ToggleAsync(string clientId, int propertyId);
        Task<List<int>> GetFavoritePropertyIdsAsync(string clientId);
        Task<List<PropertyDto>> GetFavoritePropertiesAsync(string clientId, PropertyFilterDto filter);
    }
}