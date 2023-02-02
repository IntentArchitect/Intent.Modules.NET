using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Core.InheritanceAssociations
{
    public class DerivedClassForConcreteConfiguration : IEntityTypeConfiguration<DerivedClassForConcrete>
    {
        public void Configure(EntityTypeBuilder<DerivedClassForConcrete> builder)
        {
            builder.HasBaseType<ConcreteBaseClass>();

            builder.HasPartitionKey(x => x.PartitionKey);

            builder.Property(x => x.DerivedAttribute)
                .IsRequired();
        }
    }
}