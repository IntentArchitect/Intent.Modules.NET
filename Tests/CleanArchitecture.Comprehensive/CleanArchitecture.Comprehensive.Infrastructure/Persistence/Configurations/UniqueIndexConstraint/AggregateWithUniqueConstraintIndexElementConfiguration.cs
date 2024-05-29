using CleanArchitecture.Comprehensive.Domain.Entities.UniqueIndexConstraint;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Infrastructure.Persistence.Configurations.UniqueIndexConstraint
{
    public class AggregateWithUniqueConstraintIndexElementConfiguration : IEntityTypeConfiguration<AggregateWithUniqueConstraintIndexElement>
    {
        public void Configure(EntityTypeBuilder<AggregateWithUniqueConstraintIndexElement> builder)
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
                .IsUnique();

            builder.HasIndex(x => new { x.CompUniqueFieldA, x.CompUniqueFieldB })
                .IsUnique();

            builder.Ignore(e => e.DomainEvents);
        }
    }
}