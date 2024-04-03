using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SqlServerImporterTests.Domain.Entities.Views;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace SqlServerImporterTests.Infrastructure.Persistence.Configurations.Views
{
    public class VwOrderConfiguration : IEntityTypeConfiguration<VwOrder>
    {
        public void Configure(EntityTypeBuilder<VwOrder> builder)
        {
            builder.ToView("VwOrders", "views");

            builder.HasNoKey();

            builder.Property(x => x.Id)
                .IsRequired();

            builder.Property(x => x.CustomerId)
                .IsRequired();

            builder.Property(x => x.OrderDate)
                .IsRequired();

            builder.Property(x => x.RefNo)
                .IsRequired()
                .HasColumnType("nvarchar(450)");

            builder.Ignore(e => e.DomainEvents);
        }
    }
}