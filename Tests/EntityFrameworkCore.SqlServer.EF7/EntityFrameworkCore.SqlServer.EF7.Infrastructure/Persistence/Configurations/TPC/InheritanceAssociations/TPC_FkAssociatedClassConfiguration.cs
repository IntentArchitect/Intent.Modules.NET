using EntityFrameworkCore.SqlServer.EF7.Domain.Entities.TPC.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Infrastructure.Persistence.Configurations.TPC.InheritanceAssociations
{
    public class TPC_FkAssociatedClassConfiguration : IEntityTypeConfiguration<TPC_FkAssociatedClass>
    {
        public void Configure(EntityTypeBuilder<TPC_FkAssociatedClass> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.AssociatedField)
                .IsRequired();

            builder.Property(x => x.FkDerivedClassCompositeKeyA)
                .IsRequired();

            builder.Property(x => x.FkDerivedClassCompositeKeyB)
                .IsRequired();

            builder.HasOne(x => x.FkDerivedClass)
                .WithMany()
                .HasForeignKey(x => new { x.FkDerivedClassCompositeKeyA, x.FkDerivedClassCompositeKeyB })
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}