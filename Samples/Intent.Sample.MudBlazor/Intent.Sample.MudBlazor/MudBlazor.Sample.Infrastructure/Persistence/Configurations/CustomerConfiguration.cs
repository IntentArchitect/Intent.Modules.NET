using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MudBlazor.Sample.Domain;
using MudBlazor.Sample.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace MudBlazor.Sample.Infrastructure.Persistence.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(40);

            builder.Property(x => x.AccountNo)
                .HasMaxLength(20);

            builder.OwnsOne(x => x.Address, ConfigureAddress)
                .Navigation(x => x.Address).IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }

        public static void ConfigureAddress(OwnedNavigationBuilder<Customer, Address> builder)
        {
            builder.WithOwner();

            builder.Property(x => x.Line1)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Line2)
                .HasMaxLength(200);

            builder.Property(x => x.City)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Country)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Postal)
                .IsRequired()
                .HasMaxLength(20);
        }
    }
}