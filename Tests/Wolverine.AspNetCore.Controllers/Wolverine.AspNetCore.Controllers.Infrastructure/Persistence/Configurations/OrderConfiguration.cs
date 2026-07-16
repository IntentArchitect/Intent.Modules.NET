using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wolverine.AspNetCore.Controllers.Domain;
using Wolverine.AspNetCore.Controllers.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace Wolverine.AspNetCore.Controllers.Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.OrderNumber)
                .IsRequired();

            builder.Property(x => x.Status)
                .IsRequired();

            builder.Property(x => x.CustomerName)
                .IsRequired();

            builder.Property(x => x.PlacedDate)
                .IsRequired();

            builder.Property(x => x.Notes);

            builder.OwnsMany(x => x.OrderItems, ConfigureOrderItems);

            builder.OwnsOne(x => x.ShippingAddress, ConfigureShippingAddress)
                .Navigation(x => x.ShippingAddress).IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }

        public static void ConfigureOrderItems(OwnedNavigationBuilder<Order, OrderItem> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.OrderId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ProductId)
                .IsRequired();

            builder.Property(x => x.Quantity)
                .IsRequired();

            builder.Property(x => x.UnitPrice)
                .IsRequired();

            builder.Property(x => x.OrderId)
                .IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }

        public static void ConfigureShippingAddress(OwnedNavigationBuilder<Order, ShippingAddress> builder)
        {
            builder.WithOwner();

            builder.Property(x => x.Line1)
                .IsRequired();

            builder.Property(x => x.City)
                .IsRequired();

            builder.Property(x => x.PostalCode)
                .IsRequired();

            builder.Property(x => x.Country)
                .IsRequired();
        }
    }
}