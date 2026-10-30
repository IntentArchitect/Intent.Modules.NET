using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestMediator.Publisher.Domain;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace TestMediator.Publisher.Infrastructure.Persistence.Configurations
{
    public class AuditEntryConfiguration : IEntityTypeConfiguration<AuditEntry>
    {
        public void Configure(EntityTypeBuilder<AuditEntry> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Message)
                    .IsRequired();

            builder.Property(x => x.OrderId)
                    .IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }
    }
}