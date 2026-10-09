using CqrsSpike.Subscriber.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CqrsSpike.Subscriber.Infrastructure.Persistence.Configurations
{
    public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
    {
        public void Configure(EntityTypeBuilder<Shipment> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.OrderId)
                    .IsRequired();

            builder.Property(x => x.Status)
                    .IsRequired();

            builder.Property(x => x.CreatedAt)
                    .IsRequired();
        }
    }
}