using AdvancedMappingCrud.DbContext.ProjectTo.Tests.Domain.Entities.ManyToMany;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.ProjectTo.Tests.Infrastructure.Persistence.Configurations.ManyToMany
{
    public class ProductItemConfiguration : IEntityTypeConfiguration<ProductItem>
    {
        public void Configure(EntityTypeBuilder<ProductItem> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.HasMany(x => x.Categories)
                .WithMany("Products")
                .UsingEntity(x => x.ToTable("ProductItemCategory"));

            builder.HasMany(x => x.Tags)
                .WithMany(x => x.ProductItems)
                .UsingEntity(x => x.ToTable("ProductItemTag"));

            builder.Ignore(e => e.DomainEvents);
        }
    }
}