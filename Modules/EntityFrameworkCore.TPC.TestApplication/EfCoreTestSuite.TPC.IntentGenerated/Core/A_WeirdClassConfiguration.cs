using System;
using EfCoreTestSuite.TPC.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.TPC.IntentGenerated.Core
{
    public class A_WeirdClassConfiguration : IEntityTypeConfiguration<A_WeirdClass>
    {
        public void Configure(EntityTypeBuilder<A_WeirdClass> builder)
        {
            builder.ToTable("A_WeirdClass");

            builder.Property(x => x.WeirdField)
                .IsRequired();

            builder.Ignore(e => e.DomainEvents);

        }
    }
}