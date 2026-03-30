using FluentValidationTest.Domain.Entities.ValidationScenarios.IdentityConstraints;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace FluentValidationTest.Infrastructure.Persistence.Configurations.ValidationScenarios.IdentityConstraints
{
    public class UniquePersonEntityConfiguration : IEntityTypeConfiguration<UniquePersonEntity>
    {
        public void Configure(EntityTypeBuilder<UniquePersonEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.FirstName)
                .IsRequired();

            builder.Property(x => x.LastName)
                .IsRequired();

            builder.Property(x => x.ContactNumber);

            builder.HasIndex(x => new { x.FirstName, x.LastName })
                .IsUnique()
                .HasDatabaseName("UX_UniquePerson_Name");
        }
    }
}