using GraphQL.CQRS.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace GraphQL.CQRS.TestApplication.Infrastructure.Persistence.Configurations
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.No)
                .IsRequired();

            builder.Property(x => x.Created)
                .IsRequired();

            builder.Property(x => x.CustomerId)
                .IsRequired();

            builder.OwnsMany(x => x.InvoiceLines, ConfigureInvoiceLines);

            builder.HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public void ConfigureInvoiceLines(OwnedNavigationBuilder<Invoice, InvoiceLine> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.InvoiceId);

            builder.HasKey(x => new { x.ProductId, x.InvoiceId });

            builder.Property(x => x.No)
                .IsRequired();

            builder.Property(x => x.Quantity)
                .IsRequired();

            builder.Property(x => x.Amount)
                .IsRequired();

            builder.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}