using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Scalar.Domain.Configuration;
using JsonPatchRfc7396.Scalar.Domain.Entities.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace JsonPatchRfc7396.Scalar.Infrastructure.Persistence.Configurations.Configuration
{
    public class ConfigurationChangeConfiguration : IEntityTypeConfiguration<ConfigurationChange>
    {
        public void Configure(EntityTypeBuilder<ConfigurationChange> builder)
        {
            builder.HasKey(x => x.Id);

            builder.OwnsOne(x => x.Key, ConfigureKey)
                .Navigation(x => x.Key).IsRequired();

            builder.OwnsOne(x => x.ScopeKey, ConfigureScopeKey)
                .Navigation(x => x.ScopeKey).IsRequired();

            builder.Property(x => x.OldValue);

            builder.Property(x => x.NewValue);

            builder.Property(x => x.ChangedAtUtc)
                .IsRequired();

            builder.Property(x => x.ChangedBy)
                .IsRequired()
                .HasComment(@"Identifier of the actor making the change (e.g. user id / service principal).");

            builder.Property(x => x.ConfigurationStoreId)
                .IsRequired();
        }

        public static void ConfigureKey(OwnedNavigationBuilder<ConfigurationChange, ConfigurationKey> builder)
        {
            builder.Property(x => x.Value)
                .IsRequired();
        }

        public static void ConfigureScopeKey(OwnedNavigationBuilder<ConfigurationChange, ConfigurationScopeKey> builder)
        {
            builder.Property(x => x.Scope)
                .IsRequired();

            builder.Property(x => x.ScopeId)
                .HasComment(@"Optional identifier within the scope (e.g. TenantId). Null for `Global`.");
        }
    }
}