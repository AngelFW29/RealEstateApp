using RealEstateApp.Core.Domain.Common;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;
using RealEstateApp.Infrastructure.Persistence.Contexts;

namespace RealEstateApp.Infrastructure.Persistence.Repositories
{
    public class ImprovementRepository : GenericRepository<Improvement>, IImprovementRepository
    {
        public ImprovementRepository(RealEstateAppContext context) : base(context) { }

        public Task<bool> ExistsByNameAsync(string nombre, int? excludeId = null)
        {
            throw new NotImplementedException();
        }

        public Task<List<CatalogItemWithCount<Improvement>>> GetAllWithCountAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}