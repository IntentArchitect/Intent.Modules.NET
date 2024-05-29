using CleanArchitecture.Comprehensive.Domain.DDD;
using CleanArchitecture.Comprehensive.Domain.Entities.DDD;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Infrastructure.Persistence.Configurations.DDD
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasBaseType<Person>();

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.OwnsOne(x => x.Address, ConfigureAddress)
                .Navigation(x => x.Address).IsRequired();
        }

        public void ConfigureAddress(OwnedNavigationBuilder<Customer, Address> builder)
        {
            builder.WithOwner();

            builder.Property(x => x.Line1)
                .IsRequired();

            builder.Property(x => x.Line2)
                .IsRequired();

            builder.Property(x => x.City)
                .IsRequired();

            builder.Property(x => x.Postal)
                .IsRequired();
        }
    }
}