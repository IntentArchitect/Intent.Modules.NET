using FluentValidationTest.Domain.Entities.ValidationScenarios.EnumMapping;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace FluentValidationTest.Infrastructure.Persistence.Configurations.ValidationScenarios.EnumMapping
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.StatusText)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Notes)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.ProcessText)
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}