using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestMediator.NoEventing.Domain.Products;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace TestMediator.NoEventing.Infrastructure.Persistence.Configurations.Products
{
    public class PriceHistoryEntryConfiguration : IEntityTypeConfiguration<PriceHistoryEntry>
    {
        public void Configure(EntityTypeBuilder<PriceHistoryEntry> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ProductId)
                    .IsRequired();

            builder.Property(x => x.OldPrice)
                    .IsRequired();

            builder.Property(x => x.NewPrice)
                    .IsRequired();

            builder.Property(x => x.ChangedAt)
                    .IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }
    }
}