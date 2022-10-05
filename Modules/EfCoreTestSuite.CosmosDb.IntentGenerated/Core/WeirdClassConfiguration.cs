using System;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Inheritance;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Core
{
    public class WeirdClassConfiguration : IEntityTypeConfiguration<WeirdClass>
    {
        public void Configure(EntityTypeBuilder<WeirdClass> builder)
        {
            builder.ToTable("WeirdClass");

            builder.Property(x => x.PartitionKey)
                .IsRequired();

            builder.Property(x => x.WeirdField)
                .IsRequired();
            builder.HasPartitionKey(x => x.PartitionKey);
        }
    }
}