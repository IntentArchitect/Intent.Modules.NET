using System;
using EfCoreTestSuite.TPC.IntentGenerated.Entities.Polymorphic;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.TPC.IntentGenerated.Core.Polymorphic
{
    public class Poly_TopLevelConfiguration : IEntityTypeConfiguration<Poly_TopLevel>
    {
        public void Configure(EntityTypeBuilder<Poly_TopLevel> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.TopField)
                .IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }
    }
}