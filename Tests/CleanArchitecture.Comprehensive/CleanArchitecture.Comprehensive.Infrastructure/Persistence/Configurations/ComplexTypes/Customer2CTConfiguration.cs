using CleanArchitecture.Comprehensive.Domain.ComplexTypes;
using CleanArchitecture.Comprehensive.Domain.Entities.ComplexTypes;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Infrastructure.Persistence.Configurations.ComplexTypes
{
    public class Customer2CTConfiguration : IEntityTypeConfiguration<Customer2CT>
    {
        public void Configure(EntityTypeBuilder<Customer2CT> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.OwnsOne(x => x.Address2CT, ConfigureAddress2CT)
                .Navigation(x => x.Address2CT).IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }

        public void ConfigureAddress2CT(OwnedNavigationBuilder<Customer2CT, Address2CT> builder)
        {
            builder.WithOwner();

            builder.Property(x => x.Line1)
                .IsRequired();

            builder.Property(x => x.Line2)
                .IsRequired();

            builder.Property(x => x.City)
                .IsRequired();
        }
    }
}