using System;
using EfCoreTestSuite.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Core
{
    public class D_OptionalAggregateConfiguration : IEntityTypeConfiguration<D_OptionalAggregate>
    {
        public void Configure(EntityTypeBuilder<D_OptionalAggregate> builder)
        {
            builder.HasKey(x => x.Id);


            builder.HasMany(x => x.D_MultipleDependents)
                .WithOne(x => x.D_OptionalAggregate)
                .HasForeignKey(x => x.D_OptionalAggregateId);

        }
    }
}