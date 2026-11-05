namespace RealEstateApp.Core.Application.Interfaces
{
    public interface IGenericService<TDto> where TDto : class
    {
        Task<TDto> AddAsync(TDto dto);
        Task UpdateAsync(int id, TDto dto);
        Task DeleteAsync(int id);
        Task<TDto?> GetByIdAsync(int id);
        Task<List<TDto>> GetAllAsync();
    }
}