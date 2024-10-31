using CleanArchitecture.Comprehensive.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Infrastructure.Persistence.Configurations
{
    public class ThreeConfiguration : IEntityTypeConfiguration<Three>
    {
        public void Configure(EntityTypeBuilder<Three> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.OneId)
                .IsRequired();

            builder.Property(x => x.TwoId)
                .IsRequired();

            builder.Property(x => x.ThreeId)
                .IsRequired();

            builder.HasMany(x => x.Finals)
                .WithMany("Threes")
                .UsingEntity(x => x.ToTable("ThreeFinals"));

            builder.Ignore(e => e.DomainEvents);
        }
    }
}