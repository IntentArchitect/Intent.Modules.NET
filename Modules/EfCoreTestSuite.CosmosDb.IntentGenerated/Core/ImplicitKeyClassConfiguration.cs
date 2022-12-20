using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Core
{
    public class ImplicitKeyClassConfiguration : IEntityTypeConfiguration<ImplicitKeyClass>
    {
        public void Configure(EntityTypeBuilder<ImplicitKeyClass> builder)
        {
            builder.ToContainer("PartitionKeyDefaullt");

            builder.HasPartitionKey(x => x.Id);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Attribute)
                .IsRequired();
        }
    }
}