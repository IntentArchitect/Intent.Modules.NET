using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.BasicAudit;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Infrastructure.Persistence.Configurations.BasicAudit
{
    public class Audit_SoloClassConfiguration : IEntityTypeConfiguration<Audit_SoloClass>
    {
        public void Configure(EntityTypeBuilder<Audit_SoloClass> builder)
        {
            builder.ToContainer("PartitionKeyNamed");

            builder.HasPartitionKey(x => x.PartitionKey);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PartitionKey)
                .IsRequired();

            builder.Property(x => x.SoloAttr)
                .IsRequired();

            builder.Property(x => x.CreatedBy);

            builder.Property(x => x.CreatedDate)
                .IsRequired();

            builder.Property(x => x.UpdatedBy);

            builder.Property(x => x.UpdatedDate);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}