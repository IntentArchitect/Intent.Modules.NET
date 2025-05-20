using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.OData.SimpleKey;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Infrastructure.Persistence.Configurations.OData.SimpleKey
{
    public class ODataCustomerConfiguration : IEntityTypeConfiguration<ODataCustomer>
    {
        public void Configure(EntityTypeBuilder<ODataCustomer> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Property(x => x.Surname)
                .IsRequired();

            builder.OwnsMany(x => x.ODataOrders, ConfigureODataOrders);

            builder.Ignore(e => e.DomainEvents);
        }

        public static void ConfigureODataOrders(OwnedNavigationBuilder<ODataCustomer, ODataOrder> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.CustomerId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Description)
                .IsRequired();

            builder.Property(x => x.DateOfOrder)
                .IsRequired();

            builder.Property(x => x.CustomerId)
                .IsRequired();

            builder.OwnsMany(x => x.ODataOrderLines, ConfigureODataOrderLines);
        }

        public static void ConfigureODataOrderLines(OwnedNavigationBuilder<ODataOrder, ODataOrderLine> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.OrderId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ProductName)
                .IsRequired();

            builder.Property(x => x.Quantity)
                .IsRequired();

            builder.Property(x => x.OrderId)
                .IsRequired();

            builder.Property(x => x.ODataProductId)
                .IsRequired();

            builder.HasOne(x => x.ODataProduct)
                .WithMany()
                .HasForeignKey(x => x.ODataProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}