using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Infrastructure.Persistence.Configurations
{
    public class StandaloneDerivedConfiguration : IEntityTypeConfiguration<StandaloneDerived>
    {
        public void Configure(EntityTypeBuilder<StandaloneDerived> builder)
        {
            builder.ToContainer("PartitionKeyNamed");

            builder.HasPartitionKey(x => x.PartitionKey);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PartitionKey)
                .IsRequired();

            builder.Property(x => x.BaseAttribute)
                .IsRequired();

            builder.Property(x => x.DerivedAttribute)
                .IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }
    }
}