using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Infrastructure.Persistence.EntityConfigurations
{
    public class PropertyConfiguration : IEntityTypeConfiguration<Property>
    {
        public void Configure(EntityTypeBuilder<Property> builder)
        {
            #region Basic configuration
            builder.HasKey(p => p.Id);
            builder.ToTable("Properties");
            #endregion

            #region Property & Index configurations
            builder.Property(p => p.Code).IsRequired().HasMaxLength(6);
            builder.Property(p => p.Description).IsRequired().HasMaxLength(2000);
            builder.Property(p => p.AgentId).IsRequired().HasMaxLength(450);
            
            builder.Property(p => p.Price).HasPrecision(18, 2);
            builder.Property(p => p.SizeInMeters).HasPrecision(10, 2);

            builder.HasIndex(p => p.Code).IsUnique();
            #endregion

            #region Relationships
            builder.HasOne(x => x.PropertyType)
                .WithMany()
                .HasForeignKey(x => x.PropertyTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.SaleType)
                .WithMany()
                .HasForeignKey(x => x.SaleTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Images)
                .WithOne(x => x.Property)
                .HasForeignKey(x => x.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Improvements)
                .WithOne(x => x.Property)
                .HasForeignKey(x => x.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Favorites)
                .WithOne(x => x.Property)
                .HasForeignKey(x => x.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Messages)
                .WithOne(x => x.Property)
                .HasForeignKey(x => x.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Offers)
                .WithOne(x => x.Property)
                .HasForeignKey(x => x.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion
        }
    }
}