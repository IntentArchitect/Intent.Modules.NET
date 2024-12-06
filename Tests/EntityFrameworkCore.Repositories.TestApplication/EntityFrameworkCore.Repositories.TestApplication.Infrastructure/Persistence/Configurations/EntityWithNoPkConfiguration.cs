using EntityFrameworkCore.Repositories.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Persistence.Configurations
{
    public class EntityWithNoPkConfiguration : IEntityTypeConfiguration<EntityWithNoPk>
    {
        public void Configure(EntityTypeBuilder<EntityWithNoPk> builder)
        {
            builder.HasNoKey();

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }
    }
}