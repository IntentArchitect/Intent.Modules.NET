using EntityFrameworkCore.MySql.Domain.Entities.ValueObjects;
using EntityFrameworkCore.MySql.Domain.ValueObjects;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Infrastructure.Persistence.Configurations.ValueObjects
{
    public class DictionaryWithKvPSerializedConfiguration : IEntityTypeConfiguration<DictionaryWithKvPSerialized>
    {
        public void Configure(EntityTypeBuilder<DictionaryWithKvPSerialized> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                .IsRequired();

            builder.OwnsMany(x => x.KeyValuePairSerializeds, ConfigureKeyValuePairSerializeds);
        }

        public static void ConfigureKeyValuePairSerializeds(OwnedNavigationBuilder<DictionaryWithKvPSerialized, KeyValuePairSerialized> builder)
        {
            builder.WithOwner();

            builder.ToJson();

            builder.Property(x => x.Key)
                .IsRequired();

            builder.Property(x => x.Value)
                .IsRequired();
        }
    }
}