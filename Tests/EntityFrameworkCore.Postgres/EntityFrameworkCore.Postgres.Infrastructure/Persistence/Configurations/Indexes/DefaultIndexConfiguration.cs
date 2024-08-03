using EntityFrameworkCore.Postgres.Domain.Entities.Indexes;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Infrastructure.Persistence.Configurations.Indexes
{
    public class DefaultIndexConfiguration : IEntityTypeConfiguration<DefaultIndex>
    {
        public void Configure(EntityTypeBuilder<DefaultIndex> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.IndexField)
                .IsRequired()
                .HasMaxLength(250);

            builder.HasIndex(x => x.IndexField);
        }
    }
}