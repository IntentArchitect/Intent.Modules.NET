using CqrsSpike.Publisher.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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