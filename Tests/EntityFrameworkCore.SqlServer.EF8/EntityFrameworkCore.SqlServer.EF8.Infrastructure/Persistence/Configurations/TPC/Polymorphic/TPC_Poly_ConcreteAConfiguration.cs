using EntityFrameworkCore.SqlServer.EF8.Domain.Entities.TPC.Polymorphic;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Infrastructure.Persistence.Configurations.TPC.Polymorphic
{
    public class TPC_Poly_ConcreteAConfiguration : IEntityTypeConfiguration<TPC_Poly_ConcreteA>
    {
        public void Configure(EntityTypeBuilder<TPC_Poly_ConcreteA> builder)
        {
            builder.ToTable("TpcPoly_ConcreteA");

            builder.Property(x => x.ConcreteField)
                .IsRequired();
        }
    }
}