using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Domain.Common;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;
using RealEstateApp.Infrastructure.Persistence.Contexts;

namespace RealEstateApp.Infrastructure.Persistence.Repositories
{
    public class PropertyTypeRepository : CatalogRepository<PropertyType>, IPropertyTypeRepository
    {
        public PropertyTypeRepository(RealEstateAppContext context) : base(context)
        {
        }

        public async Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.PropertyTypes.AnyAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<bool> AnyExistsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.PropertyTypes.AnyAsync(cancellationToken);
        }

        public override async Task<List<CatalogItemWithCount<PropertyType>>> GetAllWithCountAsync(CancellationToken cancellationToken = default)
        {
            return await _context.PropertyTypes
                .Select(pt => new CatalogItemWithCount<PropertyType>(
                    pt,
                    _context.Properties.Count(p => p.PropertyTypeId == pt.Id)
                ))
                .ToListAsync(cancellationToken);
        }
    }
}