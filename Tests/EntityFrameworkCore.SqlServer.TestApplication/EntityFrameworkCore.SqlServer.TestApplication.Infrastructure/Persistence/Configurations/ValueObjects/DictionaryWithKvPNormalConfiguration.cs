using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.ValueObjects;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.ValueObjects;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Persistence.Configurations.ValueObjects
{
    public class DictionaryWithKvPNormalConfiguration : IEntityTypeConfiguration<DictionaryWithKvPNormal>
    {
        public void Configure(EntityTypeBuilder<DictionaryWithKvPNormal> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                .IsRequired();

            builder.OwnsMany(x => x.KeyValuePairNormals, ConfigureKeyValuePairNormals);

            builder.Ignore(e => e.DomainEvents);
        }

        public void ConfigureKeyValuePairNormals(OwnedNavigationBuilder<DictionaryWithKvPNormal, KeyValuePairNormal> builder)
        {
            builder.WithOwner();

            builder.Property(x => x.Key)
                .IsRequired();

            builder.Property(x => x.Value)
                .IsRequired();
        }
    }
}