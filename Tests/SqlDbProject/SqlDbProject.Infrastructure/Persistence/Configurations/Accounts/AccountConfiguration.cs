using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SqlDbProject.Domain.Entities.Accounts;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace SqlDbProject.Infrastructure.Persistence.Configurations.Accounts
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("Accounts", "finance");

            builder.HasKey(x => x.AccountId);

            builder.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.AccountNumber)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(x => x.ExternalReference)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.AccountTypeId)
                .IsRequired();

            builder.Property(x => x.AccountHolderId)
                .IsRequired();

            builder.HasOne(x => x.AccountType)
                .WithMany()
                .HasForeignKey(x => x.AccountTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Stakeholder)
                .WithMany()
                .HasForeignKey(x => x.AccountHolderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}