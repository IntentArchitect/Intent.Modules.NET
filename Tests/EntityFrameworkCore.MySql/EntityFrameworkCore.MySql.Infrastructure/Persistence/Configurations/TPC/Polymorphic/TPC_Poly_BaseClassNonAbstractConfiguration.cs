using EntityFrameworkCore.MySql.Domain.Entities.TPC.Polymorphic;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Infrastructure.Persistence.Configurations.TPC.Polymorphic
{
    public class TPC_Poly_BaseClassNonAbstractConfiguration : IEntityTypeConfiguration<TPC_Poly_BaseClassNonAbstract>
    {
        public void Configure(EntityTypeBuilder<TPC_Poly_BaseClassNonAbstract> builder)
        {
            builder.ToTable("TpcPoly_BaseClassNonAbstract");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.AbstractField)
                .IsRequired();

            builder.Property(x => x.Poly_RootAbstract_AggrId);

            builder.Property(x => x.BaseField)
                .IsRequired();

            builder.Property(x => x.Poly_SecondLevelId);

            builder.HasOne(x => x.Poly_RootAbstract_Aggr)
                .WithMany()
                .HasForeignKey(x => x.Poly_RootAbstract_AggrId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.OwnsOne(x => x.Poly_RootAbstract_Comp, ConfigurePoly_RootAbstract_Comp);
        }

        public void ConfigurePoly_RootAbstract_Comp(OwnedNavigationBuilder<TPC_Poly_BaseClassNonAbstract, TPC_Poly_RootAbstract_Comp> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.Id);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CompField)
                .IsRequired();
        }
    }
}