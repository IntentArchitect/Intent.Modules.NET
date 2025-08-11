using EfCoreSoftDelete.Domain;
using EfCoreSoftDelete.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreSoftDelete.Infrastructure.Persistence.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Property(x => x.IsDeleted)
                .IsRequired();

            builder.OwnsOne(x => x.PrimaryBuilding, ConfigurePrimaryBuilding);

            builder.OwnsMany(x => x.OtherAddresses, ConfigureOtherAddresses);

            builder.OwnsOne(x => x.PrimaryAddress, ConfigurePrimaryAddress)
                .Navigation(x => x.PrimaryAddress).IsRequired();

            builder.HasQueryFilter(t => t.IsDeleted == false);
        }

        public static void ConfigureOtherAddresses(OwnedNavigationBuilder<Customer, Address> builder)
        {
            builder.WithOwner();

            builder.Property(x => x.Line1)
                .IsRequired();

            builder.Property(x => x.Line2)
                .IsRequired();

            builder.Property(x => x.City)
                .IsRequired();

            builder.Property(x => x.Postal)
                .IsRequired();

            builder.OwnsMany(x => x.OtherBuildings, ConfigureOtherBuildings);

            builder.OwnsOne(x => x.PrimaryBuilding, ConfigurePrimaryBuilding)
                .Navigation(x => x.PrimaryBuilding).IsRequired();
        }

        public static void ConfigureOtherBuildings(OwnedNavigationBuilder<Address, AddressBuilding> builder)
        {
            builder.Property(x => x.Name)
                .IsRequired();
        }

        public static void ConfigurePrimaryBuilding(OwnedNavigationBuilder<Customer, AddressBuilding> builder)
        {
            builder.Property(x => x.Name)
                .IsRequired();
        }

        public static void ConfigurePrimaryBuilding(OwnedNavigationBuilder<Address, AddressBuilding> builder)
        {
            builder.Property(x => x.Name)
                .IsRequired();
        }

        public static void ConfigurePrimaryAddress(OwnedNavigationBuilder<Customer, Address> builder)
        {
            builder.WithOwner();

            builder.Property(x => x.Line1)
                .IsRequired();

            builder.Property(x => x.Line2)
                .IsRequired();

            builder.Property(x => x.City)
                .IsRequired();

            builder.Property(x => x.Postal)
                .IsRequired();

            builder.OwnsMany(x => x.OtherBuildings, ConfigureOtherBuildings);

            builder.OwnsOne(x => x.PrimaryBuilding, ConfigurePrimaryBuilding)
                .Navigation(x => x.PrimaryBuilding).IsRequired();
        }
    }
}