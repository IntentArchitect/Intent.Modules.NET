using System;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Core
{
    public class D_MultipleDependentConfiguration : IEntityTypeConfiguration<D_MultipleDependent>
    {
        public void Configure(EntityTypeBuilder<D_MultipleDependent> builder)
        {
            builder.ToTable("D_MultipleDependent");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.MultipleDependentAttr)
                .IsRequired();

            builder.Property(x => x.PartitionKey)
                .IsRequired();
            builder.HasPartitionKey(x => x.PartitionKey);

        }
    }
}