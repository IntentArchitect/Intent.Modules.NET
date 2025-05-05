using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SqlDbProject.Domain.Entities.Accounts;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace SqlDbProject.Infrastructure.Persistence.Configurations.Accounts
{
    public class AccountTypeConfiguration : IEntityTypeConfiguration<AccountType>
    {
        public void Configure(EntityTypeBuilder<AccountType> builder)
        {
            builder.ToTable("AccountTypes", "finance");

            builder.HasKey(x => x.AccountTypeId);

            builder.Property(x => x.AccountTypeId)
                .ValueGeneratedNever();

            builder.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(255);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}