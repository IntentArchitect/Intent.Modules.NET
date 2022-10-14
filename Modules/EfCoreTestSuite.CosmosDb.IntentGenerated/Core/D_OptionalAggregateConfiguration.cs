using System;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Core
{
    public class D_OptionalAggregateConfiguration : IEntityTypeConfiguration<D_OptionalAggregate>
    {
        public void Configure(EntityTypeBuilder<D_OptionalAggregate> builder)
        {
            builder.ToContainer("EntityFrameworkCore.CosmosDb.TestApplication");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PartitionKey)
                .IsRequired();

            builder.Property(x => x.OptionalAggregateAttr)
                .IsRequired();
            builder.HasPartitionKey(x => x.PartitionKey);

            builder.HasMany(x => x.D_MultipleDependents)
                .WithOne()
                .HasForeignKey(x => x.D_OptionalAggregateId);
        }
    }
}