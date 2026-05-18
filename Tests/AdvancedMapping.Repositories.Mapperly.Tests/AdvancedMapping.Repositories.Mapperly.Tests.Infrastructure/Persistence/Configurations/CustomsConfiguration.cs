using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Infrastructure.Persistence.Configurations
{
    public class CustomsConfiguration : IEntityTypeConfiguration<Customs>
    {
        public void Configure(EntityTypeBuilder<Customs> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.OriginCountry)
                .IsRequired();

            builder.Property(x => x.DestinationCountry)
                .IsRequired();

            builder.OwnsMany(x => x.CustomsDocuments, ConfigureCustomsDocuments);
        }

        public static void ConfigureCustomsDocuments(OwnedNavigationBuilder<Customs, CustomsDocument> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.CustomsId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CustomsId)
                .IsRequired();

            builder.Property(x => x.DocumentNumber)
                .IsRequired();

            builder.Property(x => x.DocumentType)
                .IsRequired();
        }
    }
}