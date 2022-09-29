using System;
using EfCoreRepositoryTestSuite.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreRepositoryTestSuite.IntentGenerated.Core
{
    public class AggregateRoot3AggCollectionConfiguration : IEntityTypeConfiguration<AggregateRoot3AggCollection>
    {
        public void Configure(EntityTypeBuilder<AggregateRoot3AggCollection> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.AggregateRoot3Single)
                .WithMany()
                .HasForeignKey(x => x.AggregateRoot3SingleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.AggregateRoot3Nullable)
                .WithMany()
                .HasForeignKey(x => x.AggregateRoot3NullableId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.AggregateRoot3Collections)
                .WithMany("AggregateRoot3AggCollections")
                .UsingEntity(x => x.ToTable("AggregateRoot3AggCollectionAggregateRoot3Collections"));
        }
    }
}