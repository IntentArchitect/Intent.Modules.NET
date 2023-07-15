using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.BasicAudit;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Infrastructure.Persistence.Configurations.BasicAudit
{
    public class Audit_DerivedClassConfiguration : IEntityTypeConfiguration<Audit_DerivedClass>
    {
        public void Configure(EntityTypeBuilder<Audit_DerivedClass> builder)
        {
            builder.ToContainer("PartitionKeyNamed");

            builder.HasPartitionKey(x => x.PartitionKey);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PartitionKey)
                .IsRequired();

            builder.Property(x => x.CreatedBy)
                .IsRequired();

            builder.Property(x => x.CreatedDate)
                .IsRequired();

            builder.Property(x => x.UpdatedBy);

            builder.Property(x => x.UpdatedDate);

            builder.Property(x => x.DerivedAttr)
                .IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }
    }
}