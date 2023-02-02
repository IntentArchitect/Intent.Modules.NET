using System;
using EfCoreTestSuite.TPH.IntentGenerated.Entities.Polymorphic;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.TPH.IntentGenerated.Core.Polymorphic
{
    public class Poly_BaseClassNonAbstractConfiguration : IEntityTypeConfiguration<Poly_BaseClassNonAbstract>
    {
        public void Configure(EntityTypeBuilder<Poly_BaseClassNonAbstract> builder)
        {
            builder.HasBaseType<Poly_RootAbstract>();

            builder.Property(x => x.BaseField)
                .IsRequired();
        }
    }
}