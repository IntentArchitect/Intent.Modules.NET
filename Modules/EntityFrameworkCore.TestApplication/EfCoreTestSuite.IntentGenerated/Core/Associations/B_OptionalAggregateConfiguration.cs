using System;
using EfCoreTestSuite.IntentGenerated.Entities;
using EfCoreTestSuite.IntentGenerated.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Core.Associations
{
    public class B_OptionalAggregateConfiguration : IEntityTypeConfiguration<B_OptionalAggregate>
    {
        public void Configure(EntityTypeBuilder<B_OptionalAggregate> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.OptionalAggrAttr)
                .IsRequired();

            builder.HasOne(x => x.B_OptionalDependent)
                .WithOne()
                .HasForeignKey<B_OptionalAggregate>(x => x.B_OptionalDependentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}