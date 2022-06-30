using System;
using EfCoreRepositoryTestSuite.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreRepositoryTestSuite.IntentGenerated.Core
{
    public class AggregateRoot5Configuration : IEntityTypeConfiguration<AggregateRoot5>
    {
        public void Configure(EntityTypeBuilder<AggregateRoot5> builder)
        {
            builder.HasKey(x => x.Id);


            builder.HasOne(x => x.AggregateRoot5EntityWithRepo)
                .WithOne()
                .HasForeignKey<AggregateRoot5>(x => x.Id)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}