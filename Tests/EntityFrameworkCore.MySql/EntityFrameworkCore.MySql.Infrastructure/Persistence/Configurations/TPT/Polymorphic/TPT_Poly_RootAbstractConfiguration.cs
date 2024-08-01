using EntityFrameworkCore.MySql.Domain.Entities.TPT.Polymorphic;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Infrastructure.Persistence.Configurations.TPT.Polymorphic
{
    public class TPT_Poly_RootAbstractConfiguration : IEntityTypeConfiguration<TPT_Poly_RootAbstract>
    {
        public void Configure(EntityTypeBuilder<TPT_Poly_RootAbstract> builder)
        {
            builder.ToTable("TptPoly_RootAbstract");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.AbstractField)
                .IsRequired();

            builder.Property(x => x.Poly_TopLevelId);

            builder.Property(x => x.Poly_RootAbstract_AggrId);

            builder.HasOne(x => x.Poly_RootAbstract_Aggr)
                .WithMany()
                .HasForeignKey(x => x.Poly_RootAbstract_AggrId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Poly_RootAbstract_Comp)
                .WithOne()
                .HasForeignKey<TPT_Poly_RootAbstract_Comp>(x => x.Id)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}