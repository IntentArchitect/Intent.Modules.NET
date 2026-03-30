using FluentValidationTest.Domain.Entities.ValidationScenarios.TextConstraints;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace FluentValidationTest.Infrastructure.Persistence.Configurations.ValidationScenarios.TextConstraints
{
    public class TextConstrainedEntityConfiguration : IEntityTypeConfiguration<TextConstrainedEntity>
    {
        public void Configure(EntityTypeBuilder<TextConstrainedEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ShortCode)
                .IsRequired()
                .HasMaxLength(8);

            builder.Property(x => x.DisplayName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Description);

            builder.Property(x => x.RequiredName)
                .IsRequired();

            builder.Property(x => x.OptionalNotes);

            builder.Property(x => x.NullButRequired);

            builder.Property(x => x.DefaultIntButRequired)
                .IsRequired();
        }
    }
}