using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.ValueObjects;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.ValueObjects;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Infrastructure.Persistence.Configurations.ValueObjects
{
    public class DictionaryWithKvPSerializedConfiguration : IEntityTypeConfiguration<DictionaryWithKvPSerialized>
    {
        public void Configure(EntityTypeBuilder<DictionaryWithKvPSerialized> builder)
        {
            builder.ToContainer("PartitionKeyNamed");

            builder.HasPartitionKey(x => x.PartitionKey);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PartitionKey)
                .IsRequired();

            builder.Property(x => x.Title)
                .IsRequired();

            builder.OwnsMany(x => x.KeyValuePairSerializeds, ConfigureKeyValuePairSerializeds);

            builder.Ignore(e => e.DomainEvents);
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