using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WolverineEventing.Subscribe.RabbitMQ.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace WolverineEventing.Subscribe.RabbitMQ.Infrastructure.Persistence.Configurations
{
    public class ShippedOrderRecordConfiguration : IEntityTypeConfiguration<ShippedOrderRecord>
    {
        public void Configure(EntityTypeBuilder<ShippedOrderRecord> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.OrderId)
                    .IsRequired();

            builder.Property(x => x.ShippedAt)
                    .IsRequired();
        }
    }
}