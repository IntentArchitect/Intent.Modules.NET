using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Infrastructure.Persistence.Configurations.Associations
{
    public class Q_DestNameDiffConfiguration : IEntityTypeConfiguration<Q_DestNameDiff>
    {
        public void Configure(EntityTypeBuilder<Q_DestNameDiff> builder)
        {
            builder.ToContainer("PartitionKeyNamed");

            builder.HasPartitionKey(x => x.PartitionKey);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PartitionKey)
                .IsRequired();

            builder.OwnsOne(x => x.DestNameDiff, ConfigureDestNameDiff)
                .Navigation(x => x.DestNameDiff).IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }

        public void ConfigureDestNameDiff(OwnedNavigationBuilder<Q_DestNameDiff, Q_DestNameDiffDependent> builder)
        {
            builder.WithOwner();

            builder.HasKey(x => x.Id);
        }
    }
}