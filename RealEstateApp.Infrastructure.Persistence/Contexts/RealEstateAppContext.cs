using System.Reflection;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Infrastructure.Persistence.Contexts
{
    public class RealEstateAppContext : DbContext
    {
        public RealEstateAppContext(DbContextOptions<RealEstateAppContext> options) : base(options){}
        public DbSet<Propiedad> Propiedades { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
