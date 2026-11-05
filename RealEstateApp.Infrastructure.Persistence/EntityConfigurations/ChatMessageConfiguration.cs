using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Infrastructure.Persistence.EntityConfigurations
{
    public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
    {
        public void Configure(EntityTypeBuilder<ChatMessage> builder)
        {
            #region Basic configuration
            builder.HasKey(cm => cm.Id);
            builder.ToTable("ChatMessages");
            #endregion

            #region Property & Index configurations
            builder.Property(cm => cm.Content).IsRequired().HasMaxLength(1000);
            builder.Property(cm => cm.ClientId).IsRequired().HasMaxLength(450);
            builder.Property(cm => cm.AgentId).IsRequired().HasMaxLength(450);
            builder.Property(cm => cm.SenderId).IsRequired().HasMaxLength(450);

            builder.HasIndex(cm => new { cm.PropertyId, cm.ClientId });
            #endregion
        }
    }
}