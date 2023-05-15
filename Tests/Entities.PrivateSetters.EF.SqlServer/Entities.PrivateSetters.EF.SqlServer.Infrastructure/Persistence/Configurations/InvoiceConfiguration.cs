using Entities.PrivateSetters.EF.SqlServer.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace Entities.PrivateSetters.EF.SqlServer.Infrastructure.Persistence.Configurations
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Date)
                .IsRequired();

            builder.OwnsMany(x => x.Lines, ConfigureLines);

            builder.HasMany(x => x.Tags)
                .WithMany("Invoices")
                .UsingEntity(x => x.ToTable("InvoiceTags"));
        }

        public void ConfigureLines(OwnedNavigationBuilder<Invoice, Line> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.InvoiceId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.InvoiceId)
                .IsRequired();

            builder.Property(x => x.Description)
                .IsRequired();

            builder.Property(x => x.Quantity)
                .IsRequired();
        }
    }
}