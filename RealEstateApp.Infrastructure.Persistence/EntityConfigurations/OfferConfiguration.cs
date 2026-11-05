using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Infrastructure.Persistence.EntityConfigurations
{
    public class OfferConfiguration : IEntityTypeConfiguration<Offer>
    {
        public void Configure(EntityTypeBuilder<Offer> builder)
        {
            #region Basic configuration
            builder.HasKey(o => o.Id);
            builder.ToTable("Offers");
            #endregion

            #region Property & Index configurations
            builder.Property(o => o.Amount).HasPrecision(18, 2);           
            builder.Property(o => o.ClientId).IsRequired().HasMaxLength(450);                        

            builder.HasIndex(o => new { o.PropertyId, o.ClientId });
            #endregion
        }
    }
}