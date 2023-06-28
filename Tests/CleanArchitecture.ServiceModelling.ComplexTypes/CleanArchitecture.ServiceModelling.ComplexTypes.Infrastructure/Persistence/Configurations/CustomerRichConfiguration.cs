using CleanArchitecture.ServiceModelling.ComplexTypes.Domain;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Infrastructure.Persistence.Configurations
{
    public class CustomerRichConfiguration : IEntityTypeConfiguration<CustomerRich>
    {
        public void Configure(EntityTypeBuilder<CustomerRich> builder)
        {
            builder.HasKey(x => x.Id);

            builder.OwnsOne(x => x.Address, ConfigureAddress)
                .Navigation(x => x.Address).IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }

        public void ConfigureAddress(OwnedNavigationBuilder<CustomerRich, Address> builder)
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