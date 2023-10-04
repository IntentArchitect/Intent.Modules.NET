using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Infrastructure.Persistence.Configurations
{
    public class SelfContainedEntityConfiguration : IEntityTypeConfiguration<SelfContainedEntity>
    {
        public void Configure(EntityTypeBuilder<SelfContainedEntity> builder)
        {
            builder.ToContainer("SelfContained");

            builder.HasPartitionKey(x => x.SelfPartKey);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.SelfPartKey)
                .IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }
    }
}