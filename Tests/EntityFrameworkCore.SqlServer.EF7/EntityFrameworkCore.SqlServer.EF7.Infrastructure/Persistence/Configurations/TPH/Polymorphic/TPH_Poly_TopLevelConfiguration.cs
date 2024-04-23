using EntityFrameworkCore.SqlServer.EF7.Domain.Entities.TPH.Polymorphic;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Infrastructure.Persistence.Configurations.TPH.Polymorphic
{
    public class TPH_Poly_TopLevelConfiguration : IEntityTypeConfiguration<TPH_Poly_TopLevel>
    {
        public void Configure(EntityTypeBuilder<TPH_Poly_TopLevel> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.TopField)
                .IsRequired();

            builder.HasMany(x => x.RootAbstracts)
                .WithOne()
                .HasForeignKey(x => x.Poly_TopLevelId);
        }
    }
}