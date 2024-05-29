using CleanArchitecture.Comprehensive.Domain.ComplexTypes;
using CleanArchitecture.Comprehensive.Domain.Entities.ComplexTypes;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Infrastructure.Persistence.Configurations.ComplexTypes
{
    public class OrderCTConfiguration : IEntityTypeConfiguration<OrderCT>
    {
        public void Configure(EntityTypeBuilder<OrderCT> builder)
        {
            builder.HasKey(x => x.Id);

            builder.OwnsMany(x => x.LineItemCTS, ConfigureLineItemCTS);

            builder.Ignore(e => e.DomainEvents);
        }

        public void ConfigureLineItemCTS(OwnedNavigationBuilder<OrderCT, LineItemCT> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.OrderCTId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.OrderCTId)
                .IsRequired();

            builder.OwnsOne(x => x.Money, ConfigureMoney)
                .Navigation(x => x.Money).IsRequired();
        }

        public void ConfigureMoney(OwnedNavigationBuilder<LineItemCT, MoneyCT> builder)
        {
            builder.Property(x => x.Amount)
                .IsRequired();

            builder.Property(x => x.Currency)
                .IsRequired();
        }
    }
}