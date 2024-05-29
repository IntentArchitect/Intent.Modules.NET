using CleanArchitecture.Comprehensive.Domain.DDD;
using CleanArchitecture.Comprehensive.Domain.Entities.DDD;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Infrastructure.Persistence.Configurations.DDD
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasKey(x => x.Id);

            builder.OwnsOne(x => x.Current, ConfigureCurrent)
                .Navigation(x => x.Current).IsRequired();

            builder.Property(x => x.Description)
                .IsRequired();

            builder.Property(x => x.AccountId)
                .IsRequired();

            builder.HasOne(x => x.Account)
                .WithMany()
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(e => e.DomainEvents);
        }

        public void ConfigureCurrent(OwnedNavigationBuilder<Transaction, Money> builder)
        {
            builder.Property(x => x.Currency)
                .IsRequired();

            builder.Property(x => x.Amount)
                .IsRequired();
        }
    }
}