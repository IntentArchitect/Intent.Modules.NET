using EntityFrameworkCore.SqlServer.EF8.Domain.Entities.TPC.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Infrastructure.Persistence.Configurations.TPC.InheritanceAssociations
{
    public class TPC_DerivedClassForConcreteAssociatedConfiguration : IEntityTypeConfiguration<TPC_DerivedClassForConcreteAssociated>
    {
        public void Configure(EntityTypeBuilder<TPC_DerivedClassForConcreteAssociated> builder)
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