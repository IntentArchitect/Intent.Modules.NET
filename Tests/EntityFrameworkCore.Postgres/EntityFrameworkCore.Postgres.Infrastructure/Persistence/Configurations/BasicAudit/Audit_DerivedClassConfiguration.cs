using EntityFrameworkCore.Postgres.Domain.Entities.BasicAudit;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Infrastructure.Persistence.Configurations.BasicAudit
{
    public class Audit_DerivedClassConfiguration : IEntityTypeConfiguration<Audit_DerivedClass>
    {
        public void Configure(EntityTypeBuilder<Audit_DerivedClass> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CreatedBy)
                .IsRequired();

            builder.Property(x => x.CreatedDate)
                .IsRequired();

            builder.Property(x => x.UpdatedBy);

            builder.Property(x => x.UpdatedDate);

            builder.Property(x => x.DerivedAttr)
                .IsRequired();
        }
    }
}