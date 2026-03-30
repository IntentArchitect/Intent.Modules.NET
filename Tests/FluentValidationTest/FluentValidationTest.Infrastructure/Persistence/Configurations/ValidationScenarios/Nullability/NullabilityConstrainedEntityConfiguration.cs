using FluentValidationTest.Domain.Entities.ValidationScenarios.Nullability;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace FluentValidationTest.Infrastructure.Persistence.Configurations.ValidationScenarios.Nullability
{
    public class NullabilityConstrainedEntityConfiguration : IEntityTypeConfiguration<NullabilityConstrainedEntity>
    {
        public void Configure(EntityTypeBuilder<NullabilityConstrainedEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.RequiredString)
                .IsRequired();

            builder.Property(x => x.OptionalString);

            builder.Property(x => x.RequiredInt)
                .IsRequired();

            builder.Property(x => x.OptionalInt);

            builder.Property(x => x.RequiredGuidValue)
                .IsRequired();

            builder.Property(x => x.OptionalGuidValue);

            builder.Property(x => x.RequiredDateValue)
                .IsRequired();

            builder.Property(x => x.OptionalDateValue);
        }
    }
}