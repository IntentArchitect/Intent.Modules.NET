using EntityFrameworkCore.SqlServer.EF10.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF10.Infrastructure.Persistence.Configurations
{
    public class TemporalProductConfiguration : IEntityTypeConfiguration<TemporalProduct>
    {
        public void Configure(EntityTypeBuilder<TemporalProduct> builder)
        {
            builder.ToTable(tb =>
            {
                tb.IsTemporal(t =>
                {
                    t.HasPeriodStart("StartDate");
                    t.HasPeriodEnd("EndDate");
                    t.UseHistoryTable("ProductArchive");
                });
            });

            builder.HasKey(x => x.Id1);

            builder.Property(x => x.Id)
                .IsRequired();

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Property(x => x.Price)
                .IsRequired();
        }
    }
}