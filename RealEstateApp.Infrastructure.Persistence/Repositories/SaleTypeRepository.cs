using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Domain.Common;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;
using RealEstateApp.Infrastructure.Persistence.Contexts;

namespace RealEstateApp.Infrastructure.Persistence.Repositories
{
    public class SaleTypeRepository : CatalogRepository<SaleType>, ISaleTypeRepository
    {
        public SaleTypeRepository(RealEstateAppContext context) : base(context) { }

        public async Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.SaleTypes.AnyAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<bool> AnyExistsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaleTypes.AnyAsync(cancellationToken);
        }
        public override async Task<List<CatalogItemWithCount<SaleType>>> GetAllWithCountAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaleTypes
                .Select(pt => new CatalogItemWithCount<SaleType>(
                    pt,
                    _context.Properties.Count(p => p.SaleTypeId == pt.Id)
                ))
                .ToListAsync(cancellationToken);
        }
    }
}