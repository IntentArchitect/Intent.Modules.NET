using CleanArchitecture.Comprehensive.Domain.Entities.UniqueIndexConstraint;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Infrastructure.Persistence.Configurations.UniqueIndexConstraint
{
    public class AggregateWithUniqueConstraintIndexStereotypeConfiguration : IEntityTypeConfiguration<AggregateWithUniqueConstraintIndexStereotype>
    {
        public void Configure(EntityTypeBuilder<AggregateWithUniqueConstraintIndexStereotype> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.SingleUniqueField)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(x => x.CompUniqueFieldA)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(x => x.CompUniqueFieldB)
                .IsRequired()
                .HasMaxLength(256);

            builder.HasIndex(x => x.SingleUniqueField)
                .IsUnique()
                .HasDatabaseName("IX_Stereotype_SingleUniqueField");

            builder.HasIndex(x => new { x.CompUniqueFieldA, x.CompUniqueFieldB })
                .IsUnique()
                .HasDatabaseName("IX_Stereotype_CompUniqueField");

            builder.Ignore(e => e.DomainEvents);
        }
    }
}