using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Infrastructure.Persistence.Configurations
{
    public class AlternateConnStrDefaultDbDomainPackageAuditLogConfiguration : IEntityTypeConfiguration<AlternateConnStrDefaultDbDomainPackageAuditLog>
    {
        public void Configure(EntityTypeBuilder<AlternateConnStrDefaultDbDomainPackageAuditLog> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.TableName)
                .IsRequired();

            builder.Property(x => x.Key)
                .IsRequired();

            builder.Property(x => x.ColumnName);

            builder.Property(x => x.OldValue);

            builder.Property(x => x.NewValue);

            builder.Property(x => x.ChangedBy)
                .IsRequired();

            builder.Property(x => x.ChangedDate)
                .IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }
    }
}