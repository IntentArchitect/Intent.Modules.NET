using System;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Core.Associations
{
    public class J_MultipleAggregateConfiguration : IEntityTypeConfiguration<J_MultipleAggregate>
    {
        public void Configure(EntityTypeBuilder<J_MultipleAggregate> builder)
        {
            builder.ToContainer("PartitionKeyNamed");

            builder.HasPartitionKey(x => x.PartitionKey);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PartitionKey)
                .IsRequired();

            builder.Property(x => x.MultipleAggrAttr)
                .IsRequired();

            builder.HasOne(x => x.JRequiredDependent)
                .WithMany()
                .HasForeignKey(x => x.JRequiredDependentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}