using Intent.RoslynWeaver.Attributes;
using MassTransit.RabbitMQ.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace MassTransit.RabbitMQ.Infrastructure.Persistence.Configurations
{
    public class AnimalConfiguration : IEntityTypeConfiguration<Animal>
    {
        public void Configure(EntityTypeBuilder<Animal> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Property(x => x.Type)
                .IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }
    }
}