using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Domain.Common;
using RealEstateApp.Core.Domain.Interfaces;
using RealEstateApp.Infrastructure.Persistence.Contexts;
using System.Linq.Expressions;

namespace RealEstateApp.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity>
        where TEntity : BasicEntity<int>
    {
        protected readonly RealEstateAppContext _context;

        public GenericRepository(RealEstateAppContext context)
        {
            _context = context;
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task UpdateAsync(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync();
        }
        public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter = null)
        {
            return await BuildQuery(filter).CountAsync();
        }
        public async Task<TEntity?> GetByIdAsync(int id, params string[] includes)
        {
            return await BuildQuery(e => e.Id == id, includes).FirstOrDefaultAsync();
        }
        public async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null, params string[] includes)
        {
            return await BuildQuery(filter, includes).ToListAsync();
        }

        #region Private method
        private IQueryable<TEntity> BuildQuery(Expression<Func<TEntity, bool>>? filter, params string[] includes)
        {
            var query = _context.Set<TEntity>().AsQueryable();

            foreach (var include in includes)
                query = query.Include(include);

            return filter is null ? query : query.Where(filter);
        }

        #endregion
    }
}