using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Infrastructure.Persistence.EntityConfigurations
{
    public class PropertyImprovementConfiguration : IEntityTypeConfiguration<PropertyImprovement>
    {
        public void Configure(EntityTypeBuilder<PropertyImprovement> builder)
        {
            #region Basic configuration
            builder.HasKey(pi => pi.Id);
            builder.ToTable("PropertyImprovements");
            #endregion

            builder.HasIndex(pi => new { pi.PropertyId, pi.ImprovementId }).IsUnique();

            #region Relationship
            builder.HasOne(x => x.Improvement)
                .WithMany()
                .HasForeignKey(x => x.ImprovementId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion
        }
    }
}