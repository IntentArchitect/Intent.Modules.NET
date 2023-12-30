using AdvancedMappingCrud.Repositories.Tests.Domain;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.RefNo)
                .IsRequired();

            builder.Property(x => x.OrderDate)
                .IsRequired();

            builder.Property(x => x.OrderStatus)
                .IsRequired();

            builder.Property(x => x.CustomerId)
                .IsRequired();

            builder.HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.OwnsMany(x => x.OrderItems, ConfigureOrderItems);

            builder.OwnsOne(x => x.DeliveryAddress, ConfigureDeliveryAddress)
                .Navigation(x => x.DeliveryAddress).IsRequired();

            builder.OwnsOne(x => x.BillingAddress, ConfigureBillingAddress);

            builder.Ignore(e => e.DomainEvents);
        }

        public void ConfigureOrderItems(OwnedNavigationBuilder<Order, OrderItem> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.OrderId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Quantity)
                .IsRequired();

            builder.Property(x => x.Units)
                .IsRequired();

            builder.Property(x => x.UnitPrice)
                .IsRequired();

            builder.Property(x => x.OrderId)
                .IsRequired();

            builder.Property(x => x.ProductId)
                .IsRequired();

            builder.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public void ConfigureDeliveryAddress(OwnedNavigationBuilder<Order, Address> builder)
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
        }

        public void ConfigureBillingAddress(OwnedNavigationBuilder<Order, Address> builder)
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
        }
    }
}