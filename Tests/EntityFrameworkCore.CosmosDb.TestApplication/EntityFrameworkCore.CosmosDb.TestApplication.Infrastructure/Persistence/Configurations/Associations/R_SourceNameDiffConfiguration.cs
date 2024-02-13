using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Infrastructure.Persistence.Configurations.Associations
{
    public class R_SourceNameDiffConfiguration : IEntityTypeConfiguration<R_SourceNameDiff>
    {
        public void Configure(EntityTypeBuilder<R_SourceNameDiff> builder)
        {
            builder.ToContainer("PartitionKeyNamed");

            builder.HasPartitionKey(x => x.PartitionKey);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PartitionKey)
                .IsRequired();

            builder.OwnsOne(x => x.R_SourceNameDiffDependent, ConfigureR_SourceNameDiffDependent)
                .Navigation(x => x.R_SourceNameDiffDependent).IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }

        public void ConfigureR_SourceNameDiffDependent(OwnedNavigationBuilder<R_SourceNameDiff, R_SourceNameDiffDependent> builder)
        {
            builder.WithOwner();

            builder.HasKey(x => x.Id);
        }
    }
}