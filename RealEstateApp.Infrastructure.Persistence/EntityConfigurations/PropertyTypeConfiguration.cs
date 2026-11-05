using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Infrastructure.Persistence.EntityConfigurations
{
    public class PropertyTypeConfiguration : IEntityTypeConfiguration<PropertyType>
    {
        public void Configure(EntityTypeBuilder<PropertyType> builder)
        {
            #region Basic configuration
            builder.HasKey(p => p.Id);
            builder.ToTable("PropertyTypes");
            #endregion

            #region Property & Index configurations
            builder.Property(pt => pt.Name).IsRequired().HasMaxLength(100);
            builder.Property(pt => pt.Description).IsRequired().HasMaxLength(500);

            builder.HasIndex(pt => pt.Name).IsUnique();
            #endregion
        }
    }
}