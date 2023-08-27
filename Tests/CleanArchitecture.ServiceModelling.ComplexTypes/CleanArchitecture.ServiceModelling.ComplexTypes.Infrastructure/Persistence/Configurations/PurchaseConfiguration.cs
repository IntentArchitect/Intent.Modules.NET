using CleanArchitecture.ServiceModelling.ComplexTypes.Domain;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Infrastructure.Persistence.Configurations
{
    public class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
    {
        public void Configure(EntityTypeBuilder<Purchase> builder)
        {
            builder.HasKey(x => x.Id);

            builder.OwnsOne(x => x.Cost, ConfigureCost)
                .Navigation(x => x.Cost).IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }

        public void ConfigureCost(OwnedNavigationBuilder<Purchase, Money> builder)
        {
            builder.Property(x => x.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,4)");

            builder.Property(x => x.Currency)
                .IsRequired();
        }
    }
}