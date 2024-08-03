using EntityFrameworkCore.MySql.Domain.Entities.TPT.Polymorphic;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Infrastructure.Persistence.Configurations.TPT.Polymorphic
{
    public class TPT_Poly_SecondLevelConfiguration : IEntityTypeConfiguration<TPT_Poly_SecondLevel>
    {
        public void Configure(EntityTypeBuilder<TPT_Poly_SecondLevel> builder)
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