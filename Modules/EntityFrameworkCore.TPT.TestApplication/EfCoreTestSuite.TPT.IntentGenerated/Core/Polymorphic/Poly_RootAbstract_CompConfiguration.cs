using EfCoreTestSuite.TPT.IntentGenerated.Entities.Polymorphic;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.TPT.IntentGenerated.Core.Polymorphic
{
    public class Poly_RootAbstract_CompConfiguration : IEntityTypeConfiguration<Poly_RootAbstract_Comp>
    {
        public void Configure(EntityTypeBuilder<Poly_RootAbstract_Comp> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CompField)
                .IsRequired();
        }
    }
}