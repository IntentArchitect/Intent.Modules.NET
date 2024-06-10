using EntityFrameworkCore.SplitQueries.SqlServer.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SplitQueries.SqlServer.Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ReferenceNumber)
                .IsRequired();

            builder.Property(x => x.Created)
                .IsRequired();

            builder.HasIndex(x => x.ReferenceNumber)
                .IncludeProperties(x => new { x.Created });
        }
    }
}