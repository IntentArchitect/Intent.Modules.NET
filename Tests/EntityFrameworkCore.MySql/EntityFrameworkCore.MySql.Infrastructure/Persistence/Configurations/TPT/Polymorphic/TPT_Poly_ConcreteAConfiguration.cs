using EntityFrameworkCore.MySql.Domain.Entities.TPT.Polymorphic;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Infrastructure.Persistence.Configurations.TPT.Polymorphic
{
    public class TPT_Poly_ConcreteAConfiguration : IEntityTypeConfiguration<TPT_Poly_ConcreteA>
    {
        public void Configure(EntityTypeBuilder<TPT_Poly_ConcreteA> builder)
        {
            builder.ToTable("TptPoly_ConcreteA");

            builder.Property(x => x.ConcreteField)
                .IsRequired();
        }
    }
}