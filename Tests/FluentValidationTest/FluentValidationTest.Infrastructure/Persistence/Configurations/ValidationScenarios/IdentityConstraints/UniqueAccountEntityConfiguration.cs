using FluentValidationTest.Domain.Entities.ValidationScenarios.IdentityConstraints;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace FluentValidationTest.Infrastructure.Persistence.Configurations.ValidationScenarios.IdentityConstraints
{
    public class UniqueAccountEntityConfiguration : IEntityTypeConfiguration<UniqueAccountEntity>
    {
        public void Configure(EntityTypeBuilder<UniqueAccountEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Username)
                .IsRequired();

            builder.Property(x => x.Email)
                .IsRequired();

            builder.HasIndex(x => x.Username)
                .IsUnique()
                .HasDatabaseName("UX_UniqueAccount_Username");

            builder.HasIndex(x => x.Email)
                .IsUnique()
                .HasDatabaseName("UX_UniqueAccount_Email");
        }
    }
}