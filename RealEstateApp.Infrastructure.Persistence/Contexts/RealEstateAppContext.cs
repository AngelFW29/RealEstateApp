using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Infrastructure.Persistence.Contexts
{
    public class RealEstateAppContext : DbContext
    {
        public RealEstateAppContext(DbContextOptions<RealEstateAppContext> options) : base(options)
        {
        }

        public DbSet<Propiedad> Propiedades { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Propiedad>(entity =>
            {
                entity.Property(p => p.Precio).HasPrecision(18, 2);
                entity.Property(p => p.Tamano).HasPrecision(18, 2);
            });
        }
    }
}