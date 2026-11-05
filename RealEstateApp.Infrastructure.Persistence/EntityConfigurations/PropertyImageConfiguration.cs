using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Infrastructure.Persistence.EntityConfigurations
{
    public class PropertyImageConfiguration : IEntityTypeConfiguration<PropertyImage>
    {
        public void Configure(EntityTypeBuilder<PropertyImage> builder)
        {
            #region Basic configuration
            builder.HasKey(p => p.Id);
            builder.ToTable("PropertyImages");
            #endregion

            #region Property 
            builder.Property(pm => pm.ImagePath).IsRequired().HasMaxLength(500);
            #endregion
        }
    }
}