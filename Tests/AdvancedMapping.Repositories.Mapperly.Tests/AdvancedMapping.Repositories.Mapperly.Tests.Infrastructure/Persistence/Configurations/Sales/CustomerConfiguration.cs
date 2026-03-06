using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities.Sales;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Infrastructure.Persistence.Configurations.Sales
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Property(x => x.Email)
                .IsRequired();

            builder.Property(x => x.IsVip)
                .IsRequired();

            builder.Property(x => x.BirthDate);

            builder.Property(x => x.MetadataJson);

            builder.OwnsMany(x => x.Addresses, ConfigureAddresses);

            builder.OwnsOne(x => x.Preferences, ConfigurePreferences);
        }

        public static void ConfigureAddresses(OwnedNavigationBuilder<Customer, Address> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.CustomerId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CustomerId)
                .IsRequired();

            builder.Property(x => x.Line1)
                .IsRequired();

            builder.Property(x => x.Line2);

            builder.Property(x => x.City)
                .IsRequired();

            builder.Property(x => x.PostCode)
                .IsRequired();
        }

        public static void ConfigurePreferences(OwnedNavigationBuilder<Customer, Preferences> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.Id);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Newsletter)
                .IsRequired();

            builder.Property(x => x.Specials)
                .IsRequired();
        }
    }
}