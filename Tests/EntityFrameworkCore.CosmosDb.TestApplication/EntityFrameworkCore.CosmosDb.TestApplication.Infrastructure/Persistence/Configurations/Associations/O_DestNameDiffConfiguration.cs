using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Infrastructure.Persistence.Configurations.Associations
{
    public class O_DestNameDiffConfiguration : IEntityTypeConfiguration<O_DestNameDiff>
    {
        public void Configure(EntityTypeBuilder<O_DestNameDiff> builder)
        {
            builder.ToContainer("PartitionKeyNamed");

            builder.HasPartitionKey(x => x.PartitionKey);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PartitionKey)
                .IsRequired();

            builder.OwnsMany(x => x.DestNameDiff, ConfigureDestNameDiff);

            builder.Ignore(e => e.DomainEvents);
        }

        public void ConfigureDestNameDiff(OwnedNavigationBuilder<O_DestNameDiff, O_DestNameDiffDependent> builder)
        {
            builder.WithOwner();

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ODestnamediffId)
                .IsRequired();
        }
    }
}