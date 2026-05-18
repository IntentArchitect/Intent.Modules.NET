using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities.Sales;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Infrastructure.Persistence.Configurations.Sales
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CustomerId)
                .IsRequired();

            builder.Property(x => x.OrderDate)
                .IsRequired();

            builder.Property(x => x.RequiredBy);

            builder.Property(x => x.Status)
                .IsRequired();

            builder.Property(x => x.TotalAmount)
                .IsRequired();

            builder.HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.OwnsMany(x => x.Lines, ConfigureLines);

            builder.HasMany(x => x.Discounts)
                .WithMany("Orders")
                .UsingEntity(x => x.ToTable("OrderDiscounts"));

            builder.OwnsMany(x => x.Payments, ConfigurePayments);

            builder.OwnsMany(x => x.Shipments, ConfigureShipments);
        }

        public static void ConfigureLines(OwnedNavigationBuilder<Order, OrderLine> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.OrderId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.OrderId)
                .IsRequired();

            builder.Property(x => x.ProductId)
                .IsRequired();

            builder.Property(x => x.Quantity)
                .IsRequired();

            builder.Property(x => x.UnitPrice)
                .IsRequired();

            builder.Property(x => x.LineTotal);

            builder.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public static void ConfigurePayments(OwnedNavigationBuilder<Order, Payment> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.OrderId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.OrderId)
                .IsRequired();

            builder.Property(x => x.Method)
                .IsRequired();

            builder.Property(x => x.Amount)
                .IsRequired();

            builder.Property(x => x.PaidOn);
        }

        public static void ConfigureShipments(OwnedNavigationBuilder<Order, Shipment> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.OrderId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.OrderId)
                .IsRequired();

            builder.Property(x => x.Provider)
                .IsRequired();

            builder.Property(x => x.TrackingNumber);

            builder.Property(x => x.ShippedOn);

            builder.Property(x => x.CustomsId);

            builder.OwnsOne(x => x.Dispatch, ConfigureDispatch)
                .Navigation(x => x.Dispatch).IsRequired();

            builder.OwnsOne(x => x.Manifest, ConfigureManifest)
                .Navigation(x => x.Manifest).IsRequired();

            builder.HasOne(x => x.Customs)
                .WithMany()
                .HasForeignKey(x => x.CustomsId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public static void ConfigureDispatch(OwnedNavigationBuilder<Shipment, Dispatch> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.Id);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.OriginLocation)
                .IsRequired();

            builder.OwnsOne(x => x.Document, ConfigureDocument)
                .Navigation(x => x.Document).IsRequired();
        }

        public static void ConfigureDocument(OwnedNavigationBuilder<Dispatch, DispatchDocument> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.Id);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.DocumentNumber)
                .IsRequired();

            builder.Property(x => x.IssuedOn)
                .IsRequired();

            builder.Property(x => x.FileUrl);
        }

        public static void ConfigureDocument(OwnedNavigationBuilder<Manifest, ManifestDocument> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.Id);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.DocumentNumber)
                .IsRequired();

            builder.Property(x => x.IssuedOn)
                .IsRequired();

            builder.Property(x => x.FileUrl);
        }

        public static void ConfigureManifest(OwnedNavigationBuilder<Shipment, Manifest> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.Id);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CarrierCode)
                .IsRequired();

            builder.Property(x => x.TotalWeight)
                .IsRequired();

            builder.OwnsOne(x => x.Document, ConfigureDocument)
                .Navigation(x => x.Document).IsRequired();
        }
    }
}