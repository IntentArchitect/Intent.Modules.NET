using EntityFrameworkCore.MySql.Domain.Entities.TPH.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Infrastructure.Persistence.Configurations.TPH.InheritanceAssociations
{
    public class TPH_AbstractBaseClassAssociatedConfiguration : IEntityTypeConfiguration<TPH_AbstractBaseClassAssociated>
    {
        public void Configure(EntityTypeBuilder<TPH_AbstractBaseClassAssociated> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.AssociatedField)
                .IsRequired();

            builder.Property(x => x.AbstractBaseClassId)
                .IsRequired();

            builder.HasOne(x => x.AbstractBaseClass)
                .WithMany()
                .HasForeignKey(x => x.AbstractBaseClassId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}