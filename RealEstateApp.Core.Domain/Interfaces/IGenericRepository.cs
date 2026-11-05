using RealEstateApp.Core.Domain.Common;
using System.Linq.Expressions;

namespace RealEstateApp.Core.Domain.Interfaces
{
    public interface IGenericRepository<TEntity>
        where TEntity : BasicEntity<int>
    {
        Task<TEntity?> GetByIdAsync(int id, params string[] includes);
        Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null, params string[] includes);
        Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter = null);
        Task<TEntity> AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
    }
}