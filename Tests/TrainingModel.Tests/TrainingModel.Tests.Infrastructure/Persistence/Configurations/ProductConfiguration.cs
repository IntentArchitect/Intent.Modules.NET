using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingModel.Tests.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace TrainingModel.Tests.Infrastructure.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Property(x => x.BrandId)
                .IsRequired();

            builder.Property(x => x.Description)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.HasOne(x => x.Brand)
                .WithMany()
                .HasForeignKey(x => x.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Categories)
                .WithMany("Products")
                .UsingEntity(x => x.ToTable("ProductCategories"));

            builder.OwnsMany(x => x.Prices, ConfigurePrices);

            builder.Ignore(e => e.DomainEvents);
        }

        public void ConfigurePrices(OwnedNavigationBuilder<Product, Price> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.ProductId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ProductId)
                .IsRequired();

            builder.Property(x => x.ActiveFrom)
                .IsRequired();

            builder.Property(x => x.Amount)
                .IsRequired()
                .HasColumnType("decimal(16,4)");
        }
    }
}