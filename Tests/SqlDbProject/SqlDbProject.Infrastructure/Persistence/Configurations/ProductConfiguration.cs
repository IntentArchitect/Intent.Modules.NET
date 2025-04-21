using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SqlDbProject.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace SqlDbProject.Infrastructure.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products", "policy");

            builder.HasKey(x => x.ProductId);

            builder.Property(x => x.CountryIso)
                .IsRequired()
                .HasColumnType("CHAR(2)");

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(1024);

            builder.Property(x => x.IsDefault)
                .IsRequired();

            builder.Property(x => x.RatingMethodId)
                .IsRequired();

            builder.Property(x => x.ProductRatingConfig)
                .IsRequired()
                .HasColumnType("text")
                .HasColumnName("productRATINGConfig");

            builder.HasOne(x => x.Country)
                .WithMany()
                .HasForeignKey(x => x.CountryIso)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}