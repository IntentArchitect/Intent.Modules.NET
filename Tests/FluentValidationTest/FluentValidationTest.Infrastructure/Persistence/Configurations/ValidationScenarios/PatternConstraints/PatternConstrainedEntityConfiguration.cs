using FluentValidationTest.Domain.Entities.ValidationScenarios.PatternConstraints;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace FluentValidationTest.Infrastructure.Persistence.Configurations.ValidationScenarios.PatternConstraints
{
    public class PatternConstrainedEntityConfiguration : IEntityTypeConfiguration<PatternConstrainedEntity>
    {
        public void Configure(EntityTypeBuilder<PatternConstrainedEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.EmailAddress)
                .IsRequired();

            builder.Property(x => x.WebsiteUrl);

            builder.Property(x => x.Slug)
                .IsRequired();

            builder.Property(x => x.ReferenceNumber)
                .IsRequired();

            builder.Property(x => x.OptionalPatternText);
        }
    }
}