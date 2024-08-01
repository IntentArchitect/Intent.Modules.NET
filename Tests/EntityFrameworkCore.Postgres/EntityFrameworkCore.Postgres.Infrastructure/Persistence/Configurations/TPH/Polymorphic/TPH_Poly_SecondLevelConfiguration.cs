using EntityFrameworkCore.Postgres.Domain.Entities.TPH.Polymorphic;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Infrastructure.Persistence.Configurations.TPH.Polymorphic
{
    public class TPH_Poly_SecondLevelConfiguration : IEntityTypeConfiguration<TPH_Poly_SecondLevel>
    {
        public void Configure(EntityTypeBuilder<TPH_Poly_SecondLevel> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.SecondField)
                .IsRequired();

            builder.HasMany(x => x.BaseClassNonAbstracts)
                .WithOne()
                .HasForeignKey(x => x.Poly_SecondLevelId);
        }
    }
}