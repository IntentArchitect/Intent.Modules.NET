using EventingSubscribers.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EventingSubscribers.Infrastructure.Persistence.Configurations
{
    public class ShipTaskConfiguration : IEntityTypeConfiguration<ShipTask>
    {
        public void Configure(EntityTypeBuilder<ShipTask> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Priority)
                .IsRequired();
        }
    }
}