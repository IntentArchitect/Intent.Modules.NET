using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Subscribe.MassTransit.DomainInteractionsRepro.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace Subscribe.MassTransit.DomainInteractionsRepro.Infrastructure.Persistence.Configurations
{
    public class CatalogueConfiguration : IEntityTypeConfiguration<Catalogue>
    {
        public void Configure(EntityTypeBuilder<Catalogue> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Property(x => x.Code)
                .IsRequired();

            builder.OwnsMany(x => x.CatalogueItems, ConfigureCatalogueItems);
        }

        public static void ConfigureCatalogueItems(OwnedNavigationBuilder<Catalogue, CatalogueItem> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.CatalogueId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Property(x => x.Sequence)
                .IsRequired();

            builder.Property(x => x.CatalogueId)
                .IsRequired();
        }
    }
}