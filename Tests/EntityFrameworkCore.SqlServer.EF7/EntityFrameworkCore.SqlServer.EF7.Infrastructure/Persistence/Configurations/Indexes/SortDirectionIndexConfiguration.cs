using EntityFrameworkCore.SqlServer.EF7.Domain.Entities.Indexes;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Infrastructure.Persistence.Configurations.Indexes
{
    public class SortDirectionIndexConfiguration : IEntityTypeConfiguration<SortDirectionIndex>
    {
        public void Configure(EntityTypeBuilder<SortDirectionIndex> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.FieldA)
                .IsRequired();

            builder.Property(x => x.FieldB)
                .IsRequired();

            builder.HasIndex(x => new { x.FieldA, x.FieldB })
                .IsDescending(false, true);
        }
    }
}