using EntityFrameworkCore.SqlServer.EF8.Domain.Entities.TPT.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Infrastructure.Persistence.Configurations.TPT.InheritanceAssociations
{
    public class TPT_DerivedClassForAbstractAssociatedConfiguration : IEntityTypeConfiguration<TPT_DerivedClassForAbstractAssociated>
    {
        public void Configure(EntityTypeBuilder<TPT_DerivedClassForAbstractAssociated> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.AssociatedField)
                .IsRequired();

            builder.Property(x => x.DerivedClassForAbstractId)
                .IsRequired();

            builder.HasOne(x => x.DerivedClassForAbstract)
                .WithMany()
                .HasForeignKey(x => x.DerivedClassForAbstractId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}