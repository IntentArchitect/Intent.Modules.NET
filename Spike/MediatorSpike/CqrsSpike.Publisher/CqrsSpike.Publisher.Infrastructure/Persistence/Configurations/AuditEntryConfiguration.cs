using CqrsSpike.Publisher.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CqrsSpike.Publisher.Infrastructure.Persistence.Configurations
{
    public class AuditEntryConfiguration : IEntityTypeConfiguration<AuditEntry>
    {
        public void Configure(EntityTypeBuilder<AuditEntry> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Description)
                    .IsRequired();

            builder.Property(x => x.CreatedAt)
                    .IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }
    }
}