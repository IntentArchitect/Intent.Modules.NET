using EntityFrameworkCore.SplitQueries.PostgreSQL.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SplitQueries.PostgreSQL.Infrastructure.Persistence.Configurations
{
    public class GeometryTypeConfiguration : IEntityTypeConfiguration<GeometryType>
    {
        public void Configure(EntityTypeBuilder<GeometryType> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Point)
                .IsRequired()
                .HasColumnType("geography (point)");

            builder.Property(x => x.MultiPolygon)
                .IsRequired()
                .HasColumnType("geography (multipolygon)");
        }
    }
}