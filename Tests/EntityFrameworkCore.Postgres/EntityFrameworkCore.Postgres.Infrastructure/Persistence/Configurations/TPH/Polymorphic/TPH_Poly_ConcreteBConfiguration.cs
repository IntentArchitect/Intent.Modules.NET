using EntityFrameworkCore.Postgres.Domain.Entities.TPH.Polymorphic;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Infrastructure.Persistence.Configurations.TPH.Polymorphic
{
    public class TPH_Poly_ConcreteBConfiguration : IEntityTypeConfiguration<TPH_Poly_ConcreteB>
    {
        public void Configure(EntityTypeBuilder<TPH_Poly_ConcreteB> builder)
        {
            builder.HasBaseType<TPH_Poly_BaseClassNonAbstract>();

            builder.Property(x => x.ConcreteField)
                .IsRequired();
        }
    }
}