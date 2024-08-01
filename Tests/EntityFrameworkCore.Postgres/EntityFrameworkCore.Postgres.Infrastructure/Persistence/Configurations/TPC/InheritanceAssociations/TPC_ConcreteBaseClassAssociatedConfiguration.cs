using EntityFrameworkCore.Postgres.Domain.Entities.TPC.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Infrastructure.Persistence.Configurations.TPC.InheritanceAssociations
{
    public class TPC_ConcreteBaseClassAssociatedConfiguration : IEntityTypeConfiguration<TPC_ConcreteBaseClassAssociated>
    {
        public void Configure(EntityTypeBuilder<TPC_ConcreteBaseClassAssociated> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.AssociatedField)
                .IsRequired();

            builder.Property(x => x.ConcreteBaseClassId)
                .IsRequired();

            builder.HasOne(x => x.ConcreteBaseClass)
                .WithMany()
                .HasForeignKey(x => x.ConcreteBaseClassId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}