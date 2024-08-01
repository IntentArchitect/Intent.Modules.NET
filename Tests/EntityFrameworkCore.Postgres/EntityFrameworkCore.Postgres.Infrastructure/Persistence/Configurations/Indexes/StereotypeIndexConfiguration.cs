using EntityFrameworkCore.Postgres.Domain.Entities.Indexes;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Infrastructure.Persistence.Configurations.Indexes
{
    public class StereotypeIndexConfiguration : IEntityTypeConfiguration<StereotypeIndex>
    {
        public void Configure(EntityTypeBuilder<StereotypeIndex> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.DefaultIndexField)
                .IsRequired();

            builder.Property(x => x.CustomIndexField)
                .IsRequired();

            builder.Property(x => x.GroupedIndexFieldA)
                .IsRequired();

            builder.Property(x => x.GroupedIndexFieldB)
                .IsRequired();

            builder.HasIndex(x => x.DefaultIndexField);

            builder.HasIndex(x => x.CustomIndexField)
                .HasDatabaseName("CustomIndexField");

            builder.HasIndex(x => new { x.GroupedIndexFieldA, x.GroupedIndexFieldB })
                .IsUnique()
                .HasDatabaseName("GroupedIndexField");
        }
    }
}