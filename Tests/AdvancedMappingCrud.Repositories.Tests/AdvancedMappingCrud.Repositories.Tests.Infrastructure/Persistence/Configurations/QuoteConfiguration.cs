using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Infrastructure.Persistence.Configurations
{
    public class QuoteConfiguration : IEntityTypeConfiguration<Quote>
    {
        public void Configure(EntityTypeBuilder<Quote> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.RefNo)
                .IsRequired();

            builder.Property(x => x.PersonId)
                .IsRequired();

            builder.Property(x => x.PersonEmail);

            builder.OwnsMany(x => x.QuoteLines, ConfigureQuoteLines);

            builder.Ignore(e => e.DomainEvents);
        }

        public void ConfigureQuoteLines(OwnedNavigationBuilder<Quote, QuoteLine> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.QuoteId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.QuoteId)
                .IsRequired();

            builder.Property(x => x.ProductId)
                .IsRequired();

            builder.Property(x => x.Units)
                .IsRequired();

            builder.Property(x => x.UnitPrice)
                .IsRequired();

            builder.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}