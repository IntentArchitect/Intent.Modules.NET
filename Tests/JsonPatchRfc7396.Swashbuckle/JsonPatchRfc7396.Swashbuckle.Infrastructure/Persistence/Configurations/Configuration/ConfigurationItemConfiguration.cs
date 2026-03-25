using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Swashbuckle.Domain.Configuration;
using JsonPatchRfc7396.Swashbuckle.Domain.Entities.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Infrastructure.Persistence.Configurations.Configuration
{
    public class ConfigurationItemConfiguration : IEntityTypeConfiguration<ConfigurationItem>
    {
        public void Configure(EntityTypeBuilder<ConfigurationItem> builder)
        {
            builder.HasKey(x => x.Id);

            builder.OwnsOne(x => x.Key, ConfigureKey)
                .Navigation(x => x.Key).IsRequired();

            builder.OwnsOne(x => x.ScopeKey, ConfigureScopeKey)
                .Navigation(x => x.ScopeKey).IsRequired();

            builder.Property(x => x.ValueType)
                .IsRequired();

            builder.Property(x => x.Value)
                .IsRequired()
                .HasComment(@"Serialized representation of the value. Interpretation depends on `ValueType`.");

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.Property(x => x.Version)
                .IsRequired()
                .HasComment(@"Monotonically increasing version for cache invalidation / optimistic concurrency.");

            builder.Property(x => x.UpdatedAtUtc)
                .IsRequired();

            builder.Property(x => x.ConfigurationStoreId)
                .IsRequired();

            builder.Property(x => x.LatestChangeId);

            builder.HasOne(x => x.LatestChange)
                .WithOne()
                .HasForeignKey<ConfigurationItem>(x => x.LatestChangeId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public static void ConfigureKey(OwnedNavigationBuilder<ConfigurationItem, ConfigurationKey> builder)
        {
            builder.Property(x => x.Value)
                .IsRequired();
        }

        public static void ConfigureScopeKey(OwnedNavigationBuilder<ConfigurationItem, ConfigurationScopeKey> builder)
        {
            builder.Property(x => x.Scope)
                .IsRequired();

            builder.Property(x => x.ScopeId)
                .HasComment(@"Optional identifier within the scope (e.g. TenantId). Null for `Global`.");
        }
    }
}