using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Infrastructure.Persistence.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Property(x => x.Surname)
                .IsRequired();

            builder.Property(x => x.DateOfBirth)
                .IsRequired();

            builder.OwnsMany(x => x.Addresses, ConfigureAddresses);
        }

        public static void ConfigureAddresses(OwnedNavigationBuilder<Customer, Address> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.CustomerId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Line1)
                .IsRequired();

            builder.Property(x => x.Line2)
                .IsRequired();

            builder.Property(x => x.City)
                .IsRequired();

            builder.Property(x => x.Province)
                .IsRequired();

            builder.Property(x => x.PostalCode)
                .IsRequired();

            builder.Property(x => x.Country)
                .IsRequired();

            builder.Property(x => x.CustomerId)
                .IsRequired();
        }
    }
}