using CleanArchitecture.Comprehensive.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Infrastructure.Persistence.Configurations
{
    public class OneConfiguration : IEntityTypeConfiguration<One>
    {
        public void Configure(EntityTypeBuilder<One> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.OneId)
                .IsRequired();

            builder.HasMany(x => x.Seconds)
                .WithMany("Ones")
                .UsingEntity(x => x.ToTable("OneSeconds"));

            builder.Ignore(e => e.DomainEvents);
        }
    }
}