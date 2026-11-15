using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Domain.Common;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;
using RealEstateApp.Infrastructure.Persistence.Contexts;

namespace RealEstateApp.Infrastructure.Persistence.Repositories
{
    public class ImprovementRepository : GenericRepository<Improvement>, IImprovementRepository
    {
        public ImprovementRepository(RealEstateAppContext context) : base(context) { }
        public async Task<bool> AnyExistsAsync( CancellationToken cancellationToken = default)
        {
            return await _context.Improvements.AnyAsync(cancellationToken);

        }

        public async Task<bool> ExistsByNameAsync(string nombre, int? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return await _context.Improvements
                    .AnyAsync(i => i.Name.ToLower() == nombre.ToLower() && i.Id != excludeId.Value);
            }

            return await _context.Improvements
                .AnyAsync(i => i.Name.ToLower() == nombre.ToLower());
        }

        public async Task<List<CatalogItemWithCount<Improvement>>> GetAllWithCountAsync(CancellationToken cancellationToken = default)
        {
            var improvements = await _context.Improvements
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var result = new List<CatalogItemWithCount<Improvement>>();

            foreach (var improvement in improvements)
            {
                int count = await _context.Set<PropertyImprovement>()
                    .CountAsync(pi => pi.ImprovementId == improvement.Id, cancellationToken);

                result.Add(new CatalogItemWithCount<Improvement>(improvement, count));
            }

            return result;
        }
    }
}