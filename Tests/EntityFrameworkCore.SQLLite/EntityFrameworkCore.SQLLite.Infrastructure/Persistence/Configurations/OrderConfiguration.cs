using EntityFrameworkCore.SQLLite.Domain;
using EntityFrameworkCore.SQLLite.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SQLLite.Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.RefNo)
                .IsRequired();

            builder.OwnsOne(x => x.Total, ConfigureTotal)
                .Navigation(x => x.Total).IsRequired();

            builder.OwnsMany(x => x.QuotedAmounts, ConfigureQuotedAmounts);

            builder.OwnsMany(x => x.OrderLines, ConfigureOrderLines);
        }

        public static void ConfigureTotal(OwnedNavigationBuilder<Order, Money> builder)
        {
            builder.Property(x => x.Value)
                .IsRequired();

            builder.Property(x => x.Currency)
                .IsRequired()
                .HasMaxLength(3);
        }

        public static void ConfigureQuotedAmounts(OwnedNavigationBuilder<Order, Money> builder)
        {
            builder.HasKey("Id");

            builder.Property(x => x.Value)
                .IsRequired();

            builder.Property(x => x.Currency)
                .IsRequired()
                .HasMaxLength(3);
        }

        public static void ConfigureOrderLines(OwnedNavigationBuilder<Order, OrderLine> builder)
        {
            builder.WithOwner();

            builder.HasKey("Id");

            builder.Property(x => x.Units)
                .IsRequired();

            builder.OwnsOne(x => x.Price, ConfigurePrice)
                .Navigation(x => x.Price).IsRequired();

            builder.OwnsOne(x => x.Discount, ConfigureDiscount);
        }

        public static void ConfigurePrice(OwnedNavigationBuilder<OrderLine, Money> builder)
        {
            builder.Property(x => x.Value)
                .IsRequired();

            builder.Property(x => x.Currency)
                .IsRequired()
                .HasMaxLength(3);
        }

        public static void ConfigureDiscount(OwnedNavigationBuilder<OrderLine, Money> builder)
        {
            builder.Property(x => x.Value)
                .IsRequired();

            builder.Property(x => x.Currency)
                .IsRequired()
                .HasMaxLength(3);
        }
    }
}