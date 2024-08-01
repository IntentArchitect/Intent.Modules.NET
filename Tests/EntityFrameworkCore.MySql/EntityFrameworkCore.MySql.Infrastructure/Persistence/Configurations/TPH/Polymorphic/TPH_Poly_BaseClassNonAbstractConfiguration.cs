using EntityFrameworkCore.MySql.Domain.Entities.TPH.Polymorphic;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Infrastructure.Persistence.Configurations.TPH.Polymorphic
{
    public class TPH_Poly_BaseClassNonAbstractConfiguration : IEntityTypeConfiguration<TPH_Poly_BaseClassNonAbstract>
    {
        public void Configure(EntityTypeBuilder<TPH_Poly_BaseClassNonAbstract> builder)
        {
            builder.HasBaseType<TPH_Poly_RootAbstract>();

            builder.Property(x => x.BaseField)
                .IsRequired();

            builder.Property(x => x.Poly_SecondLevelId);
        }
    }
}