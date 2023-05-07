using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.ValueObjects;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.ValueObjects;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Infrastructure.Persistence.Configurations.ValueObjects
{
    public class PersonWithAddressNormalConfiguration : IEntityTypeConfiguration<PersonWithAddressNormal>
    {
        public void Configure(EntityTypeBuilder<PersonWithAddressNormal> builder)
        {
            builder.ToContainer("PartitionKeyNamed");

            builder.HasPartitionKey(x => x.PartitionKey);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PartitionKey)
                .IsRequired();

            builder.Property(x => x.Name)
                .IsRequired();

            builder.OwnsOne(x => x.AddressNormal, ConfigureAddressNormal)
                .Navigation(x => x.AddressNormal).IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }

        public void ConfigureAddressNormal(OwnedNavigationBuilder<PersonWithAddressNormal, AddressNormal> builder)
        {
            builder.WithOwner();

            builder.Property(x => x.Line1)
                .IsRequired();

            builder.Property(x => x.Line2)
                .IsRequired();

            builder.Property(x => x.City)
                .IsRequired();
        }
    }
}