using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RichDomain.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace RichDomain.Infrastructure.Persistence.Configurations
{
    public class EntityWithAutoAppliedNewFieldsConfiguration : IEntityTypeConfiguration<EntityWithAutoAppliedNewFields>
    {
        public void Configure(EntityTypeBuilder<EntityWithAutoAppliedNewFields> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CreatedByName)
                .IsRequired();

            builder.Property(x => x.CreatedDate)
                .IsRequired();

            builder.Property(x => x.UpdatedBy);

            builder.Property(x => x.UpdatedDate);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}