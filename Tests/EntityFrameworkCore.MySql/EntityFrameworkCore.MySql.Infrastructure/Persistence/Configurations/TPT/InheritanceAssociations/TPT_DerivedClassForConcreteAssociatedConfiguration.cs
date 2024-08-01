using EntityFrameworkCore.MySql.Domain.Entities.TPT.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Infrastructure.Persistence.Configurations.TPT.InheritanceAssociations
{
    public class TPT_DerivedClassForConcreteAssociatedConfiguration : IEntityTypeConfiguration<TPT_DerivedClassForConcreteAssociated>
    {
        public void Configure(EntityTypeBuilder<TPT_DerivedClassForConcreteAssociated> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.AssociatedField)
                .IsRequired();

            builder.Property(x => x.DerivedClassForConcreteId)
                .IsRequired();

            builder.HasOne(x => x.DerivedClassForConcrete)
                .WithMany()
                .HasForeignKey(x => x.DerivedClassForConcreteId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}