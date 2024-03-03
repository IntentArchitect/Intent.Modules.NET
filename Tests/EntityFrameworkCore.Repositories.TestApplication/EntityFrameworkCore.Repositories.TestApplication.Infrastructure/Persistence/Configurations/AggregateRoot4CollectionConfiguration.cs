using EntityFrameworkCore.Repositories.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Persistence.Configurations
{
    public class AggregateRoot4CollectionConfiguration : IEntityTypeConfiguration<AggregateRoot4Collection>
    {
        public void Configure(EntityTypeBuilder<AggregateRoot4Collection> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.AggregateRoot4AggNullableId);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}