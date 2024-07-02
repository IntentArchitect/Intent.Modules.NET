using IntegrationTesting.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace IntegrationTesting.Tests.Infrastructure.Persistence.Configurations
{
    public class HasMissingDepConfiguration : IEntityTypeConfiguration<HasMissingDep>
    {
        public void Configure(EntityTypeBuilder<HasMissingDep> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Property(x => x.MissingDepId)
                .IsRequired();

            builder.HasOne(x => x.MissingDep)
                .WithMany()
                .HasForeignKey(x => x.MissingDepId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}