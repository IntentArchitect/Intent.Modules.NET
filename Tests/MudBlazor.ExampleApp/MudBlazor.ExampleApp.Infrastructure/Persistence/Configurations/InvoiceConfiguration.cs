using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MudBlazor.ExampleApp.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace MudBlazor.ExampleApp.Infrastructure.Persistence.Configurations
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.InvoiceNo)
                .IsRequired()
                .HasMaxLength(12);

            builder.Property(x => x.IssuedDate)
                .IsRequired();

            builder.Property(x => x.DueDate)
                .IsRequired();

            builder.Property(x => x.Reference)
                .HasMaxLength(25);

            builder.Property(x => x.CustomerId)
                .IsRequired();

            builder.OwnsMany(x => x.OrderLines, ConfigureOrderLines);

            builder.HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(e => e.DomainEvents);
        }

        public static void ConfigureOrderLines(OwnedNavigationBuilder<Invoice, InvoiceLine> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.InvoiceId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.InvoiceId)
                .IsRequired();

            builder.Property(x => x.ProductId)
                .IsRequired();

            builder.Property(x => x.Units)
                .IsRequired();

            builder.Property(x => x.UnitPrice)
                .IsRequired();

            builder.Property(x => x.Discount);

            builder.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}