using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.TPT.Polymorphic;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Persistence.Configurations.TPT.Polymorphic
{
    public class Poly_ConcreteBConfiguration : IEntityTypeConfiguration<Poly_ConcreteB>
    {
        public void Configure(EntityTypeBuilder<Poly_ConcreteB> builder)
        {
            builder.ToTable("TptPoly_ConcreteB");

            builder.Property(x => x.ConcreteField)
                .IsRequired();
        }
    }
}