using CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Infrastructure.Persistence.Configurations
{
    public class TestDecimalsConfiguration : IEntityTypeConfiguration<TestDecimals>
    {
        public void Configure(EntityTypeBuilder<TestDecimals> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Price1)
                .IsRequired()
                .HasColumnType("decimal(18,4)");

            builder.Property(x => x.Price2)
                .IsRequired()
                .HasColumnType("decimal(16, 4)");

            builder.Ignore(e => e.DomainEvents);
        }
    }
}