using IntegrationTesting.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace IntegrationTesting.Tests.Infrastructure.Persistence.Configurations
{
    public class HasDateOnlyFieldConfiguration : IEntityTypeConfiguration<HasDateOnlyField>
    {
        public void Configure(EntityTypeBuilder<HasDateOnlyField> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.MyDate)
                .IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }
    }
}