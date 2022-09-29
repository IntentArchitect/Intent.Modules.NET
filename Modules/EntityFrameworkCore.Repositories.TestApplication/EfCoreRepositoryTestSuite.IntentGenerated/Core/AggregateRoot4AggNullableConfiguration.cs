using System;
using EfCoreRepositoryTestSuite.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreRepositoryTestSuite.IntentGenerated.Core
{
    public class AggregateRoot4AggNullableConfiguration : IEntityTypeConfiguration<AggregateRoot4AggNullable>
    {
        public void Configure(EntityTypeBuilder<AggregateRoot4AggNullable> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.AggregateRoot4Single)
                .WithOne()
                .HasForeignKey<AggregateRoot4AggNullable>(x => x.Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.AggregateRoot4Collections)
                .WithOne()
                .HasForeignKey(x => x.AggregateRoot4AggNullableId);

            builder.HasOne(x => x.AggregateRoot4Nullable)
                .WithOne()
                .HasForeignKey<AggregateRoot4AggNullable>(x => x.AggregateRoot4NullableId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}