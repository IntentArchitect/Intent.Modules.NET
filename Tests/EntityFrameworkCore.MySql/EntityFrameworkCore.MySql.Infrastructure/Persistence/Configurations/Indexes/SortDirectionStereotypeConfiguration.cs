using EntityFrameworkCore.MySql.Domain.Entities.Indexes;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Infrastructure.Persistence.Configurations.Indexes
{
    public class SortDirectionStereotypeConfiguration : IEntityTypeConfiguration<SortDirectionStereotype>
    {
        public void Configure(EntityTypeBuilder<SortDirectionStereotype> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.FieldA)
                .IsRequired();

            builder.Property(x => x.FieldB)
                .IsRequired();

            builder.HasIndex(x => new { x.FieldA, x.FieldB })
                .IsDescending(false, true)
                .HasDatabaseName("MyIndex");
        }
    }
}