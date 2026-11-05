using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealEstateApp.Core.Domain.Interfaces;

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
        }

        public static async Task RunPersistenceSeedAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<Contexts.RealEstateAppContext>();

            await context.Database.MigrateAsync();
        }
    }
}