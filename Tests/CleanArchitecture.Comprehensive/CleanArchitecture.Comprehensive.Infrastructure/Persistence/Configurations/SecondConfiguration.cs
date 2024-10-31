using CleanArchitecture.Comprehensive.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Infrastructure.Persistence.Configurations
{
    public class SecondConfiguration : IEntityTypeConfiguration<Second>
    {
        public void Configure(EntityTypeBuilder<Second> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.OneId)
                .IsRequired();

            builder.Property(x => x.Twoid)
                .IsRequired();

            builder.HasMany(x => x.Threes)
                .WithMany("Twos")
                .UsingEntity(x => x.ToTable("SecondThrees"));

            builder.Ignore(e => e.DomainEvents);
        }
    }
}