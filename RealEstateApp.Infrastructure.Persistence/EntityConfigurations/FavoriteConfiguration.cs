using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace RealEstateApp.Infrastructure.Persistence.EntityConfigurations
{
    public class FavoriteConfiguration : IEntityTypeConfiguration<Favorite>
    {
        public void Configure(EntityTypeBuilder<Favorite> builder)
        {
            #region Basic configuration
            builder.HasKey(f => f.Id);
            builder.ToTable("Favorites");
            #endregion

            #region Property & index configuration
            builder.Property(f => f.ClientId).IsRequired().HasMaxLength(450);

            builder.HasIndex(f => new { f.ClientId, f.PropertyId }).IsUnique();
            #endregion
        }
    }
}