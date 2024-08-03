using EntityFrameworkCore.Postgres.Domain.Entities.TPT.Polymorphic;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Infrastructure.Persistence.Configurations.TPT.Polymorphic
{
    public class TPT_Poly_TopLevelConfiguration : IEntityTypeConfiguration<TPT_Poly_TopLevel>
    {
        public void Configure(EntityTypeBuilder<TPT_Poly_TopLevel> builder)
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