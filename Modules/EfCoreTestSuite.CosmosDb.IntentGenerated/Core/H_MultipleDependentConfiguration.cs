using System;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Core
{
    public class H_MultipleDependentConfiguration : IEntityTypeConfiguration<H_MultipleDependent>
    {
        public void Configure(EntityTypeBuilder<H_MultipleDependent> builder)
        {
            builder.ToTable("H_MultipleDependent");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PartitionKey)
                .IsRequired();

            builder.Property(x => x.MultipleDepAttr)
                .IsRequired();
            builder.HasPartitionKey(x => x.PartitionKey);
        }
    }
}