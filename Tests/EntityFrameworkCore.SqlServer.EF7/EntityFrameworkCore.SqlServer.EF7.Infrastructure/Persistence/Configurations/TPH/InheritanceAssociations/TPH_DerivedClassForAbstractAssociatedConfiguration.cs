using EntityFrameworkCore.SqlServer.EF7.Domain.Entities.TPH.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Infrastructure.Persistence.Configurations.TPH.InheritanceAssociations
{
    public class TPH_DerivedClassForAbstractAssociatedConfiguration : IEntityTypeConfiguration<TPH_DerivedClassForAbstractAssociated>
    {
        public void Configure(EntityTypeBuilder<TPH_DerivedClassForAbstractAssociated> builder)
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