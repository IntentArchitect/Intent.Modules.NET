using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.Indexing;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Infrastructure.Persistence.Configurations.Indexing
{
    public class FilteredIndexConfiguration : IEntityTypeConfiguration<FilteredIndex>
    {
        public void Configure(EntityTypeBuilder<FilteredIndex> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.HasIndex(x => new { x.Name, x.IsActive })
                .HasFilter("\"IsActive\" = true");

            builder.Ignore(e => e.DomainEvents);
        }
    }
}