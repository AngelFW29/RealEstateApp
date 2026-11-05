using RealEstateApp.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace RealEstateApp.Infrastructure.Persistence.Contexts
{
    public class RealEstateAppContext : DbContext
    {
        public RealEstateAppContext(DbContextOptions<RealEstateAppContext> options) : base(options) { }

        public DbSet<Property> Properties { get; set; }
        public DbSet<PropertyImage> PropertyImages { get; set; }
        public DbSet<PropertyType> PropertyTypes { get; set; }
        public DbSet<SaleType> SaleTypes { get; set; }
        public DbSet<Improvement> Improvements { get; set; }
        public DbSet<PropertyImprovement> PropertyImprovements { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<Offer> Offers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);            
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}