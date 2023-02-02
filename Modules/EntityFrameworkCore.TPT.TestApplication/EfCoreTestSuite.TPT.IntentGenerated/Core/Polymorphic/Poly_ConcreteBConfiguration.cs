using System;
using EfCoreTestSuite.TPT.IntentGenerated.Entities.Polymorphic;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.TPT.IntentGenerated.Core.Polymorphic
{
    public class Poly_ConcreteBConfiguration : IEntityTypeConfiguration<Poly_ConcreteB>
    {
        public void Configure(EntityTypeBuilder<Poly_ConcreteB> builder)
        {
            builder.ToTable("Poly_ConcreteBs");

            builder.Property(x => x.ConcreteField)
                .IsRequired();
        }
    }
}