using EntityFrameworkCore.SqlServer.EF8.Domain.Entities.TimeConcepts;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Infrastructure.Persistence.Configurations.TimeConcepts
{
    public class TimeEntityConfiguration : IEntityTypeConfiguration<TimeEntity>
    {
        public void Configure(EntityTypeBuilder<TimeEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Date)
                .IsRequired();

            builder.Property(x => x.DateTime)
                .IsRequired();

            builder.Property(x => x.DateTimeOffset)
                .IsRequired();

            builder.Property(x => x.TimeSpan)
                .IsRequired();
        }
    }
}