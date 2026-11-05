using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Infrastructure.Persistence.EntityConfigurations
{
    public class ImprovementConfiguration : IEntityTypeConfiguration<Improvement>
    {
        public void Configure(EntityTypeBuilder<Improvement> builder)
        {
            #region Basic configuration
            builder.HasKey(i => i.Id);
            builder.ToTable("Improvements");
            #endregion

            #region Property & Index configurations
            builder.Property(i => i.Name).IsRequired().HasMaxLength(100);
            builder.Property(i => i.Description).IsRequired().HasMaxLength(500);

            builder.HasIndex(i => i.Name).IsUnique();
            #endregion
        }
    }
}