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
        }
    }
}