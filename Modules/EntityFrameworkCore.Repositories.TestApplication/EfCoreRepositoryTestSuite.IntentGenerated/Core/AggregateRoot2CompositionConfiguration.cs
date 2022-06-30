using System;
using EfCoreRepositoryTestSuite.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreRepositoryTestSuite.IntentGenerated.Core
{
    public class AggregateRoot2CompositionConfiguration : IEntityTypeConfiguration<AggregateRoot2Composition>
    {
        public void Configure(EntityTypeBuilder<AggregateRoot2Composition> builder)
        {
            builder.HasKey(x => x.Id);


            builder.HasOne(x => x.AggregateRoot2Single)
                .WithOne()
                .HasForeignKey<AggregateRoot2Composition>(x => x.Id)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.AggregateRoot2Nullable)
                .WithOne()
                .HasForeignKey<AggregateRoot2Nullable>(x => x.Id)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.AggregateRoot2Collections)
                .WithOne()
                .HasForeignKey(x => x.AggregateRoot2CompositionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}