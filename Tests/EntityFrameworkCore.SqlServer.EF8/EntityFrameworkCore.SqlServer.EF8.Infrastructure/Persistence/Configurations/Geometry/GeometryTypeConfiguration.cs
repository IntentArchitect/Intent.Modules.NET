using EntityFrameworkCore.SqlServer.EF8.Domain.Entities.Geometry;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Infrastructure.Persistence.Configurations.Geometry
{
    public class GeometryTypeConfiguration : IEntityTypeConfiguration<GeometryType>
    {
        public void Configure(EntityTypeBuilder<GeometryType> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Point)
                .IsRequired();
        }
    }
}