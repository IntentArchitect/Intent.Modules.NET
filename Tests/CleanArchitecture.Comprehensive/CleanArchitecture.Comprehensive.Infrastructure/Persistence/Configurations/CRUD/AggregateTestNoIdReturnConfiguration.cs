using CleanArchitecture.Comprehensive.Domain.Entities.CRUD;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Infrastructure.Persistence.Configurations.CRUD
{
    public class AggregateTestNoIdReturnConfiguration : IEntityTypeConfiguration<AggregateTestNoIdReturn>
    {
        public void Configure(EntityTypeBuilder<AggregateTestNoIdReturn> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Attribute)
                .IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }
    }
}