using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.TPT.Polymorphic;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Persistence.Configurations.TPT.Polymorphic
{
    public class Poly_RootAbstract_CompConfiguration : IEntityTypeConfiguration<Poly_RootAbstract_Comp>
    {
        public void Configure(EntityTypeBuilder<Poly_RootAbstract_Comp> builder)
        {
            builder.ToTable("TptPoly_RootAbstract_Comp");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CompField)
                .IsRequired();
        }
    }
}