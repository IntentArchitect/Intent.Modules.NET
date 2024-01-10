using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasBaseType<Person>();

            builder.Property(x => x.Email)
                .IsRequired();

            builder.Property(x => x.QuoteId)
                .IsRequired();

            builder.HasOne(x => x.Quote)
                .WithMany()
                .HasForeignKey(x => x.QuoteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.OwnsMany(x => x.Addresses, ConfigureAddresses);

            builder.OwnsOne(x => x.DefaultDeliveryAddress, ConfigureDefaultDeliveryAddress)
                .Navigation(x => x.DefaultDeliveryAddress).IsRequired();

            builder.OwnsOne(x => x.DefaultBillingAddress, ConfigureDefaultBillingAddress);
        }

        public void ConfigureAddresses(OwnedNavigationBuilder<User, UserAddress> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.UserId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.Line1)
                .IsRequired();

            builder.Property(x => x.Line2)
                .IsRequired();

            builder.Property(x => x.City)
                .IsRequired();

            builder.Property(x => x.Postal)
                .IsRequired();
        }

        public void ConfigureDefaultDeliveryAddress(OwnedNavigationBuilder<User, UserDefaultAddress> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.Id);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Line1)
                .IsRequired();

            builder.Property(x => x.Line2)
                .IsRequired();
        }

        public void ConfigureDefaultBillingAddress(OwnedNavigationBuilder<User, UserDefaultAddress> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.Id);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Line1)
                .IsRequired();

            builder.Property(x => x.Line2)
                .IsRequired();
        }
    }
}