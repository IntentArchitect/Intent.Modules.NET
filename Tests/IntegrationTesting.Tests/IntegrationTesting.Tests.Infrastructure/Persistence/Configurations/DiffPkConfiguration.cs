using IntegrationTesting.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace IntegrationTesting.Tests.Infrastructure.Persistence.Configurations
{
    public class DiffPkConfiguration : IEntityTypeConfiguration<DiffPk>
    {
        public void Configure(EntityTypeBuilder<DiffPk> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.NAme)
                .IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }
    }
}