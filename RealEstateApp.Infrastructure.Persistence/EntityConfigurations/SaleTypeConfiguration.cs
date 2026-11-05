using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Infrastructure.Persistence.EntityConfigurations
{
    public class SaleTypeConfiguration : IEntityTypeConfiguration<SaleType>
    {
        public void Configure(EntityTypeBuilder<SaleType> builder)
        {
            #region Basic configuration
            builder.HasKey(st => st.Id);
            builder.ToTable("SaleTypes");
            #endregion

            #region Property & Index configurations
            builder.Property(st => st.Name).IsRequired().HasMaxLength(100);
            builder.Property(st => st.Description).IsRequired().HasMaxLength(500);

            builder.HasIndex(st => st.Name).IsUnique();
            #endregion
        }
    }
}