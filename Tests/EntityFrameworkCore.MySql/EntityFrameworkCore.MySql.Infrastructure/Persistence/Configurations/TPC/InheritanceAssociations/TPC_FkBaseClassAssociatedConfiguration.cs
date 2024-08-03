using EntityFrameworkCore.MySql.Domain.Entities.TPC.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Infrastructure.Persistence.Configurations.TPC.InheritanceAssociations
{
    public class TPC_FkBaseClassAssociatedConfiguration : IEntityTypeConfiguration<TPC_FkBaseClassAssociated>
    {
        public void Configure(EntityTypeBuilder<TPC_FkBaseClassAssociated> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.AssociatedField)
                .IsRequired();

            builder.Property(x => x.FkBaseClassCompositeKeyA)
                .IsRequired();

            builder.Property(x => x.FkBaseClassCompositeKeyB)
                .IsRequired();

            builder.HasOne(x => x.FkBaseClass)
                .WithMany()
                .HasForeignKey(x => new { x.FkBaseClassCompositeKeyA, x.FkBaseClassCompositeKeyB })
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}