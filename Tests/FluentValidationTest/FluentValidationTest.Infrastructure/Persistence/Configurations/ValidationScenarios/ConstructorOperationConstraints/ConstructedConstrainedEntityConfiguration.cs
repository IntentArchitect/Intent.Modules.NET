using FluentValidationTest.Domain.Entities.ValidationScenarios.ConstructorOperationConstraints;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace FluentValidationTest.Infrastructure.Persistence.Configurations.ValidationScenarios.ConstructorOperationConstraints
{
    public class ConstructedConstrainedEntityConfiguration : IEntityTypeConfiguration<ConstructedConstrainedEntity>
    {
        public void Configure(EntityTypeBuilder<ConstructedConstrainedEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                .IsRequired();

            builder.Property(x => x.Code)
                .IsRequired();

            builder.Property(x => x.OptionalComment);
        }
    }
}