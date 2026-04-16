using FluentValidationTest.Domain.Entities.ValidationScenarios.StressSuite;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace FluentValidationTest.Infrastructure.Persistence.Configurations.ValidationScenarios.StressSuite
{
    public class PersistencePrecedenceConfiguration : IEntityTypeConfiguration<PersistencePrecedence>
    {
        public void Configure(EntityTypeBuilder<PersistencePrecedence> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.RdbmsOnly)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.DomainOnly)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.BothDefined)
                .IsRequired()
                .HasMaxLength(30);
        }
    }
}