using CleanArchitecture.SingleFiles.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Infrastructure.Persistence.Configurations
{
    public class EfInvoiceConfiguration : IEntityTypeConfiguration<EfInvoice>
    {
        public void Configure(EntityTypeBuilder<EfInvoice> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Description)
                .IsRequired();

            builder.OwnsMany(x => x.EfLines, ConfigureEfLines);

            builder.Ignore(e => e.DomainEvents);
        }

        public void ConfigureEfLines(OwnedNavigationBuilder<EfInvoice, EfLine> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.EfInvoicesId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Property(x => x.EfInvoicesId)
                .IsRequired();
        }
    }
}