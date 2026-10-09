using CqrsSpike.Publisher.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CqrsSpike.Publisher.Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CustomerName)
                    .IsRequired();

            builder.Property(x => x.Total)
                    .IsRequired();

            builder.Property(x => x.Status)
                    .IsRequired();

            builder.Property(x => x.ShipmentId);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}