using CleanArchitecture.Comprehensive.Domain.ComplexTypes;
using CleanArchitecture.Comprehensive.Domain.Entities.ComplexTypes;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Infrastructure.Persistence.Configurations.ComplexTypes
{
    public class CustomerCTConfiguration : IEntityTypeConfiguration<CustomerCT>
    {
        public void Configure(EntityTypeBuilder<CustomerCT> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.OwnsOne(x => x.AddressCT, ConfigureAddressCT)
                .Navigation(x => x.AddressCT).IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }

        public void ConfigureAddressCT(OwnedNavigationBuilder<CustomerCT, AddressCT> builder)
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