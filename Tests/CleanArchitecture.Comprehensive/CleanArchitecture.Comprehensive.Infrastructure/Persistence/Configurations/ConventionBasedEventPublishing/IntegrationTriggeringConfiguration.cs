using CleanArchitecture.Comprehensive.Domain.Entities.ConventionBasedEventPublishing;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Infrastructure.Persistence.Configurations.ConventionBasedEventPublishing
{
    public class IntegrationTriggeringConfiguration : IEntityTypeConfiguration<IntegrationTriggering>
    {
        public void Configure(EntityTypeBuilder<IntegrationTriggering> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Value)
                .IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }
    }
}