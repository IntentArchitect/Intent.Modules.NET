using CqrsSpike.Subscriber.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CqrsSpike.Subscriber.Infrastructure.Persistence.Configurations
{
    public class OrderRecordConfiguration : IEntityTypeConfiguration<OrderRecord>
    {
        public void Configure(EntityTypeBuilder<OrderRecord> builder)
        {
            builder.HasKey(x => x.Id);

            // One row per order: backs the handlers' idempotency checks against concurrent
            // redelivery. EF Core InMemory does not enforce it; a relational provider does.
            builder.HasIndex(x => x.OrderId)
                .IsUnique();

            builder.Property(x => x.CustomerName)
                    .IsRequired();

            builder.Property(x => x.Total)
                    .IsRequired();

            builder.Property(x => x.OrderId)
                    .IsRequired();
        }
    }
}