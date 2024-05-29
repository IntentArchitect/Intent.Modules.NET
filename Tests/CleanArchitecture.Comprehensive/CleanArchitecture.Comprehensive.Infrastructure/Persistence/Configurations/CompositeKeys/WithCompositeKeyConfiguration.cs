using CleanArchitecture.Comprehensive.Domain.Entities.CompositeKeys;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Infrastructure.Persistence.Configurations.CompositeKeys
{
    public class WithCompositeKeyConfiguration : IEntityTypeConfiguration<WithCompositeKey>
    {
        public void Configure(EntityTypeBuilder<WithCompositeKey> builder)
        {
            builder.HasKey(x => new { x.Key1Id, x.Key2Id });

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }
    }
}