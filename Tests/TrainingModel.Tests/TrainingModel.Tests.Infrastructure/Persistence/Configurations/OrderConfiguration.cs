using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingModel.Tests.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace TrainingModel.Tests.Infrastructure.Persistence.Configurations
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

            builder.Property(x => x.CustomerId)
                .IsRequired();

            builder.HasIndex(x => x.RefNo)
                .IsUnique();

            builder.HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.OwnsMany(x => x.OrderItems, ConfigureOrderItems);

            builder.Ignore(e => e.DomainEvents);
        }

        public void ConfigureOrderItems(OwnedNavigationBuilder<Order, OrderItem> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.OrderId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.OrderId)
                .IsRequired();

            builder.Property(x => x.Amount)
                .IsRequired()
                .HasColumnType("decimal(16,4)");

            builder.Property(x => x.Quantity)
                .IsRequired();

            builder.Property(x => x.ProductId)
                .IsRequired();

            builder.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}