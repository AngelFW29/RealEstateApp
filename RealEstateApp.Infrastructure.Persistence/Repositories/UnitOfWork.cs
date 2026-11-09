using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Domain.Interfaces;
using RealEstateApp.Infrastructure.Persistence.Contexts;

namespace RealEstateApp.Infrastructure.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly RealEstateAppContext _context;

        public UnitOfWork(RealEstateAppContext context)
        {
            _context = context;
        }

        public async Task ExecuteInTransactionAsync(Func<Task> action)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();

                await action();

                await transaction.CommitAsync();
            });
        }
    }
}