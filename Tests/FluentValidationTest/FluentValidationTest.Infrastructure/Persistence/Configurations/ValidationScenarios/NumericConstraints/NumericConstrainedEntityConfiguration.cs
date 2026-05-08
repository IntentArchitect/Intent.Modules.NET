using FluentValidationTest.Domain.Entities.ValidationScenarios.NumericConstraints;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace FluentValidationTest.Infrastructure.Persistence.Configurations.ValidationScenarios.NumericConstraints
{
    public class NumericConstrainedEntityConfiguration : IEntityTypeConfiguration<NumericConstrainedEntity>
    {
        public void Configure(EntityTypeBuilder<NumericConstrainedEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Age)
                .IsRequired();

            builder.Property(x => x.Percentage)
                .IsRequired();

            builder.Property(x => x.Score)
                .IsRequired();

            builder.Property(x => x.Price)
                .IsRequired();

            builder.Property(x => x.OptionalThreshold);

            builder.Property(x => x.ExclusiveMinInclusiveMaxFloat)
                .IsRequired()
                .HasComment(@"Tests min exclusive (>), max inclusive (<=). Value must be > 0 and <= 100.");

            builder.Property(x => x.InclusiveMinExclusiveMaxDouble)
                .IsRequired()
                .HasComment(@"Tests min inclusive (>=), max exclusive (<). Value must be >= 10 and < 50.");

            builder.Property(x => x.ExclusiveMinExclusiveMaxDecimal)
                .IsRequired()
                .HasComment(@"Tests both boundaries exclusive (> and <). Value must be > 0.01 and < 999.99.");

            builder.Property(x => x.OnlyMinExclusiveFloat)
                .IsRequired()
                .HasComment(@"Tests only min boundary defined as exclusive (>). Value must be > -10.5.");

            builder.Property(x => x.OnlyMaxExclusiveDouble)
                .IsRequired()
                .HasComment(@"Tests only max boundary defined as exclusive (<). Value must be < 1000.0.");

            builder.Property(x => x.InclusiveMinInclusiveMaxFloat)
                .IsRequired()
                .HasComment(@"Tests both boundaries inclusive (>= and <=). Value must be >= 5.5 and <= 99.9.");

            builder.Property(x => x.NegativeRangeDecimal)
                .IsRequired()
                .HasComment(@"Tests negative number range with mixed boundaries. Value must be >= -100.00 and < -0.01.");

            builder.Property(x => x.NarrowRangeDouble)
                .IsRequired()
                .HasComment(@"Tests very narrow range with exclusive boundaries. Value must be > 0.001 and < 0.999.");
        }
    }
}