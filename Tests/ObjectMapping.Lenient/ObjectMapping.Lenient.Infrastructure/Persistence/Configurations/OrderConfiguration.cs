using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ObjectMapping.Lenient.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace ObjectMapping.Lenient.Infrastructure.Persistence.Configurations
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

            builder.Property(x => x.Notes);

            builder.Property(x => x.CustomerId)
                .IsRequired();

            builder.HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.OwnsOne(x => x.Coupon, ConfigureCoupon);

            builder.OwnsMany(x => x.OrderLines, ConfigureOrderLines);

            builder.HasMany(x => x.Tags)
                .WithMany("Orders")
                .UsingEntity(x => x.ToTable("OrderTags"));

            builder.OwnsMany(x => x.PaymentMethods, ConfigurePaymentMethods);
        }

        public static void ConfigureCoupon(OwnedNavigationBuilder<Order, Coupon> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.Id);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Code)
                .IsRequired();

            builder.Property(x => x.PercentOff)
                .IsRequired();

            builder.Property(x => x.Kind)
                .IsRequired();
        }

        public static void ConfigureOrderLines(OwnedNavigationBuilder<Order, OrderLine> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.OrderId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ProductName)
                .IsRequired();

            builder.Property(x => x.Quantity)
                .IsRequired();

            builder.Property(x => x.OrderId)
                .IsRequired();
        }

        public static void ConfigurePaymentMethods(OwnedNavigationBuilder<Order, PaymentMethod> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.OrderId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Label)
                .IsRequired();

            builder.Property(x => x.OrderId)
                .IsRequired();
        }
    }
}