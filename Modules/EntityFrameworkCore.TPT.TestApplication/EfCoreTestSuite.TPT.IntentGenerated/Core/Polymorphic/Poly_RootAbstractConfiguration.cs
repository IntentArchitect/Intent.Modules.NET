using EfCoreTestSuite.TPT.IntentGenerated.Entities.Polymorphic;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.TPT.IntentGenerated.Core.Polymorphic
{
    public class Poly_RootAbstractConfiguration : IEntityTypeConfiguration<Poly_RootAbstract>
    {
        public void Configure(EntityTypeBuilder<Poly_RootAbstract> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.AbstractField)
                .IsRequired();

            builder.HasOne(x => x.PolyRootAbstractAggr)
                .WithMany()
                .HasForeignKey(x => x.PolyRootAbstractAggrId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.PolyRootAbstractComp)
                .WithOne()
                .HasForeignKey<Poly_RootAbstract_Comp>(x => x.Id)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}