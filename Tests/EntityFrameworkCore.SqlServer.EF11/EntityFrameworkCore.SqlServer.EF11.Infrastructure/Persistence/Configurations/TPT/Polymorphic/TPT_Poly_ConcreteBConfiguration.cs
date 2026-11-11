using EntityFrameworkCore.SqlServer.EF11.Domain.TPT.Polymorphic;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Infrastructure.Persistence.Configurations.TPT.Polymorphic
{
    public class TPT_Poly_ConcreteBConfiguration : IEntityTypeConfiguration<TPT_Poly_ConcreteB>
    {
        public void Configure(EntityTypeBuilder<TPT_Poly_ConcreteB> builder)
        {
            builder.ToTable("TptPoly_ConcreteB");

            builder.Property(x => x.ConcreteField)
                    .IsRequired();
        }
    }
}