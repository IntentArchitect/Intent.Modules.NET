using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Infrastructure.Persistence.Configurations.Associations
{
    public class P_SourceNameDiffConfiguration : IEntityTypeConfiguration<P_SourceNameDiff>
    {
        public void Configure(EntityTypeBuilder<P_SourceNameDiff> builder)
        {
            builder.ToContainer("PartitionKeyNamed");

            builder.HasPartitionKey(x => x.PartitionKey);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PartitionKey)
                .IsRequired();

            builder.OwnsMany(x => x.P_SourceNameDiffDependents, ConfigureP_SourceNameDiffDependents);

            builder.Ignore(e => e.DomainEvents);
        }

        public void ConfigureP_SourceNameDiffDependents(OwnedNavigationBuilder<P_SourceNameDiff, P_SourceNameDiffDependent> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.SourceNameDiffId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.SourceNameDiffId)
                .IsRequired();
        }
    }
}