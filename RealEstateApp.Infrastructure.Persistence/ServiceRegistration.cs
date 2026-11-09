using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealEstateApp.Core.Domain.Interfaces;
using RealEstateApp.Infrastructure.Persistence.Repositories;

namespace RealEstateApp.Infrastructure.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<Contexts.RealEstateAppContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    m => m.MigrationsAssembly(typeof(Contexts.RealEstateAppContext).Assembly.FullName)));

            services.AddScoped(typeof(IGenericRepository<>), typeof(Repositories.GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        public static async Task RunPersistenceSeedAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<Contexts.RealEstateAppContext>();

            await context.Database.MigrateAsync();
        }
    }
}