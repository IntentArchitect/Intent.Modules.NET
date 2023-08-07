using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Infrastructure.Persistence.Configurations.InheritanceAssociations
{
    public class MiddleAbstract_LeafConfiguration : IEntityTypeConfiguration<MiddleAbstract_Leaf>
    {
        public void Configure(EntityTypeBuilder<MiddleAbstract_Leaf> builder)
        {
            builder.ToContainer("PartitionKeyNamed");

            builder.HasPartitionKey(x => x.PartitionKey);

            builder.Property(x => x.MiddleAttribute)
                .IsRequired();

            builder.Property(x => x.LeafAttribute)
                .IsRequired();
        }
    }
}