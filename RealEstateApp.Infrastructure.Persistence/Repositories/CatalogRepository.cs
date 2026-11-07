using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Domain.Common;
using RealEstateApp.Core.Domain.Interfaces;
using RealEstateApp.Infrastructure.Persistence.Contexts;

namespace RealEstateApp.Infrastructure.Persistence.Repositories
{
    public class CatalogRepository<T> : GenericRepository<T>, ICatalogRepository<T> where T : TypeEntity
    {
        public CatalogRepository(RealEstateAppContext context) : base(context)
        {
        }

        public virtual async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        {
            var query = _context.Set<T>().AsQueryable();

            if (excludeId.HasValue)
            {
                query = query.Where(x => x.Id != excludeId.Value);
            }

            return await query.AnyAsync(x => x.Name.ToLower() == name.ToLower());
        }

        public virtual async Task<List<CatalogItemWithCount<T>>> GetAllWithCountAsync(CancellationToken cancellationToken = default)
        {
            // Se implementará en clases derivadas para manejar el conteo específico por tipo
            var items = await _context.Set<T>().ToListAsync(cancellationToken);
            return items.Select(i => new CatalogItemWithCount<T>(i, 0)).ToList();
        }
    }
}
