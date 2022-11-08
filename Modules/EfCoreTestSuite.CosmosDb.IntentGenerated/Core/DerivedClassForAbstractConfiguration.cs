using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Core
{
    public class DerivedClassForAbstractConfiguration : IEntityTypeConfiguration<DerivedClassForAbstract>
    {
        public void Configure(EntityTypeBuilder<DerivedClassForAbstract> builder)
        {
            builder.HasBaseType<AbstractBaseClass>();

            builder.HasPartitionKey(x => x.PartitionKey);

            builder.Property(x => x.DerivedAttribute)
                .IsRequired();
        }
    }
}