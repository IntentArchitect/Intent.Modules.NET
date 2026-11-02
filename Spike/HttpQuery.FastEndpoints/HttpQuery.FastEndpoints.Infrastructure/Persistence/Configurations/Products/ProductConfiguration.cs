using HttpQuery.FastEndpoints.Domain.Products;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace HttpQuery.FastEndpoints.Infrastructure.Persistence.Configurations.Products
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                    .IsRequired();

            builder.Property(x => x.Category)
                    .IsRequired();

            builder.Property(x => x.Price)
                    .IsRequired();
        }
    }
}