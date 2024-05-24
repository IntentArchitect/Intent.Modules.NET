using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ValueObjects.Class.Domain;
using ValueObjects.Class.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace ValueObjects.Class.Infrastructure.Persistence.Configurations
{
    public class TestEntityConfiguration : IEntityTypeConfiguration<TestEntity>
    {
        public void Configure(EntityTypeBuilder<TestEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.OwnsOne(x => x.Amount, ConfigureAmount)
                .Navigation(x => x.Amount).IsRequired();

            builder.OwnsOne(x => x.Address, ConfigureAddress)
                .Navigation(x => x.Address).IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }

        public void ConfigureAmount(OwnedNavigationBuilder<TestEntity, Money> builder)
        {
            builder.Property(x => x.Amount)
                .IsRequired();

            builder.Property(x => x.Currency)
                .IsRequired();
        }

        public void ConfigureAddress(OwnedNavigationBuilder<TestEntity, Address> builder)
        {
            builder.Property(x => x.Line1)
                .IsRequired();

            builder.Property(x => x.Line2)
                .IsRequired();

            builder.Property(x => x.City)
                .IsRequired();

            builder.Property(x => x.Country)
                .IsRequired();

            builder.Property(x => x.AddressType)
                .IsRequired();
        }
    }
}