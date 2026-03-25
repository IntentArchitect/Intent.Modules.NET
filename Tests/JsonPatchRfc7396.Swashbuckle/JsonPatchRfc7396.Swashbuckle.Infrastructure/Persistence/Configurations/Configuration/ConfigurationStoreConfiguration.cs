using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Swashbuckle.Domain.Entities.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Infrastructure.Persistence.Configurations.Configuration
{
    public class ConfigurationStoreConfiguration : IEntityTypeConfiguration<ConfigurationStore>
    {
        public void Configure(EntityTypeBuilder<ConfigurationStore> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasComment(@"Logical name of the configuration store (e.g. ""Default"").");

            builder.HasMany(x => x.Items)
                .WithOne()
                .HasForeignKey(x => x.ConfigurationStoreId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Changes)
                .WithOne()
                .HasForeignKey(x => x.ConfigurationStoreId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}