using EntityFrameworkCore.SqlServer.EF8.Domain.Entities.TPH.Polymorphic;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Infrastructure.Persistence.Configurations.TPH.Polymorphic
{
    public class TPH_Poly_ConcreteAConfiguration : IEntityTypeConfiguration<TPH_Poly_ConcreteA>
    {
        public void Configure(EntityTypeBuilder<TPH_Poly_ConcreteA> builder)
        {
            builder.HasBaseType<TPH_Poly_BaseClassNonAbstract>();

            builder.Property(x => x.ConcreteField)
                .IsRequired();
        }
    }
}