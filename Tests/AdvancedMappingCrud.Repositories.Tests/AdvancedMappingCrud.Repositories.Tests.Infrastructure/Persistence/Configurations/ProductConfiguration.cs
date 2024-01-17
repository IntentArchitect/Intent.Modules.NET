using AdvancedMappingCrud.Repositories.Tests.Domain;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Infrastructure.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.OwnsMany(x => x.Tags, ConfigureTags);

            builder.Ignore(e => e.DomainEvents);
        }

        public void ConfigureTags(OwnedNavigationBuilder<Product, Tag> builder)
        {
            builder.WithOwner();

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Property(x => x.Value)
                .IsRequired();
        }
    }
}