using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProxyServiceTests.OriginalServices.Domain;
using ProxyServiceTests.OriginalServices.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace ProxyServiceTests.OriginalServices.Infrastructure.Persistence.Configurations
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Number)
                .IsRequired();

            builder.OwnsOne(x => x.Amount, ConfigureAmount)
                .Navigation(x => x.Amount).IsRequired();

            builder.Property(x => x.ClientId)
                .IsRequired();

            builder.HasOne(x => x.Client)
                .WithMany()
                .HasForeignKey(x => x.ClientId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public void ConfigureAmount(OwnedNavigationBuilder<Account, Money> builder)
        {
            builder.Property(x => x.Amount)
                .IsRequired();

            builder.Property(x => x.Currency)
                .IsRequired();
        }
    }
}