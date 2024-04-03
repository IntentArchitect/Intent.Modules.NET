using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SqlServerImporterTests.Domain.Entities.Dbo;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace SqlServerImporterTests.Infrastructure.Persistence.Configurations.Dbo
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders", "dbo");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CustomerId)
                .IsRequired();

            builder.Property(x => x.OrderDate)
                .IsRequired();

            builder.Property(x => x.RefNo)
                .IsRequired()
                .HasColumnType("nvarchar(450)");

            builder.HasIndex(x => x.CustomerId)
                .IncludeProperties(x => new { x.OrderDate })
                .HasDatabaseName("IX_Orders_CustomerId");

            builder.HasIndex(x => x.RefNo)
                .IsUnique()
                .HasDatabaseName("IX_Orders_RefNo");

            builder.HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}