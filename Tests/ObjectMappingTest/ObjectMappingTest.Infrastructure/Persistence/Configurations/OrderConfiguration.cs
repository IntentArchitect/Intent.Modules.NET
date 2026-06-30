using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ObjectMappingTest.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace ObjectMappingTest.Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.RefNo)
                .IsRequired();

            builder.Property(x => x.CustomerId)
                .IsRequired();

            builder.Property(x => x.Status)
                .IsRequired();

            builder.Property(x => x.PaymentStatus)
                .IsRequired();

            builder.HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.OwnsMany(x => x.Lines, ConfigureLines);

            builder.OwnsMany(x => x.Tags, ConfigureTags);
        }

        public static void ConfigureLines(OwnedNavigationBuilder<Order, OrderLine> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.OrderId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ProductName)
                .IsRequired();

            builder.Property(x => x.Qty)
                .IsRequired();

            builder.Property(x => x.DiscountCode);

            builder.Property(x => x.UnitPrice)
                .IsRequired();

            builder.Property(x => x.OrderId)
                .IsRequired();
        }

        public static void ConfigureTags(OwnedNavigationBuilder<Order, Tag> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.OrderId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Property(x => x.OrderId)
                .IsRequired();
        }
    }
}