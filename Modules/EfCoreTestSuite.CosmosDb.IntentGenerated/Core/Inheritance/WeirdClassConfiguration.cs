using System;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Inheritance;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Core.Inheritance
{
    public class WeirdClassConfiguration : IEntityTypeConfiguration<WeirdClass>
    {
        public void Configure(EntityTypeBuilder<WeirdClass> builder)
        {
            builder.HasBaseType<Composite>();

            builder.HasPartitionKey(x => x.PartitionKey);

            builder.Property(x => x.WeirdField)
                .IsRequired();
        }
    }
}