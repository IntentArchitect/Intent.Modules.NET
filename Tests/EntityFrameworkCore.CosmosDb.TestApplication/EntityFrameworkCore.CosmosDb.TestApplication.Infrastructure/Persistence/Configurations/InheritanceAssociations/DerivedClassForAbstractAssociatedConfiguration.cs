using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Infrastructure.Persistence.Configurations.InheritanceAssociations
{
    public class DerivedClassForAbstractAssociatedConfiguration : IEntityTypeConfiguration<DerivedClassForAbstractAssociated>
    {
        public void Configure(EntityTypeBuilder<DerivedClassForAbstractAssociated> builder)
        {
            builder.ToContainer("PartitionKeyNamed");

            builder.HasPartitionKey(x => x.PartitionKey);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PartitionKey)
                .IsRequired();

            builder.Property(x => x.AssociatedField)
                .IsRequired();

            builder.Property(x => x.DerivedClassForAbstractId)
                .IsRequired();

            builder.HasOne(x => x.DerivedClassForAbstract)
                .WithMany()
                .HasForeignKey(x => x.DerivedClassForAbstractId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}