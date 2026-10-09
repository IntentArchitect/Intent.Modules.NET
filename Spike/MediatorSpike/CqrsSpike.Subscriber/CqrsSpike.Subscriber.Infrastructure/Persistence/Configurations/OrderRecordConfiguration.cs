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

            builder.Property(x => x.CustomerName)
                    .IsRequired();

            builder.Property(x => x.Total)
                    .IsRequired();

            builder.Property(x => x.OrderId)
                    .IsRequired();
        }
    }
}