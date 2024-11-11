using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Standard.AspNetCore.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Infrastructure.Persistence.Configurations
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Number)
                .IsRequired();

            builder.OwnsMany(x => x.InvoiceLines, ConfigureInvoiceLines);
        }

        public static void ConfigureInvoiceLines(OwnedNavigationBuilder<Invoice, InvoiceLine> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.InvoiceId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Description)
                .IsRequired();

            builder.Property(x => x.InvoiceId)
                .IsRequired();
        }
    }
}