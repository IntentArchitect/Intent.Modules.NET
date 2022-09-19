using System;
using EfCoreTestSuite.IntentGenerated.Entities;
using EfCoreTestSuite.IntentGenerated.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Core
{
    public class F_OptionalAggregateNavConfiguration : IEntityTypeConfiguration<F_OptionalAggregateNav>
    {
        public void Configure(EntityTypeBuilder<F_OptionalAggregateNav> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.F_OptionalDependent)
                .WithOne(x => x.F_OptionalAggregateNav)
                .HasForeignKey<F_OptionalAggregateNav>(x => x.F_OptionalDependentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}