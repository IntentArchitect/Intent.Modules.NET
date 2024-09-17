using System;
using System.Linq;
using AdvancedMappingCrud.DbContext.Tests.Domain;
using AdvancedMappingCrud.DbContext.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.Tests.Infrastructure.Persistence.Configurations
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

            builder.Property(x => x.EntityStatus)
                .IsRequired();

            builder.HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.OwnsMany(x => x.OrderItems, ConfigureOrderItems);

            builder.ToTable(tb => tb.HasCheckConstraint("order_order_status_check", $"\"OrderStatus\" IN ({string.Join(",", Enum.GetValues<OrderStatus>().Select(e => $"{e:D}"))})"));

            builder.ToTable(tb => tb.HasCheckConstraint("order_entity_status_check", $"\"EntityStatus\" IN ({string.Join(",", Enum.GetValues<EntityStatus>().Select(e => $"{e:D}"))})"));

            builder.Ignore(e => e.DomainEvents);
        }

        public void ConfigureOrderItems(OwnedNavigationBuilder<Order, OrderItem> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.OrderId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.OrderId)
                .IsRequired();

            builder.Property(x => x.Quantity)
                .IsRequired();

            builder.Property(x => x.Amount)
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