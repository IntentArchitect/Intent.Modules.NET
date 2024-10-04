using System;
using System.Linq;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingModel.Tests.Domain;
using TrainingModel.Tests.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace TrainingModel.Tests.Infrastructure.Persistence.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Property(x => x.Surname)
                .IsRequired();

            builder.Property(x => x.Email)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.OwnsOne(x => x.Preferences, ConfigurePreferences);

            builder.OwnsMany(x => x.Address, ConfigureAddress);

            builder.Ignore(e => e.DomainEvents);
        }

        public void ConfigurePreferences(OwnedNavigationBuilder<Customer, Preferences> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.Id);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Specials)
                .IsRequired();

            builder.Property(x => x.News)
                .IsRequired();
        }

        public void ConfigureAddress(OwnedNavigationBuilder<Customer, Address> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.CustomersId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Line1)
                .IsRequired();

            builder.Property(x => x.Line2)
                .IsRequired();

            builder.Property(x => x.City)
                .IsRequired();

            builder.Property(x => x.Postal)
                .IsRequired();

            builder.Property(x => x.AddressType)
                .IsRequired();

            builder.Property(x => x.CustomersId)
                .IsRequired();

            builder.ToTable(tb => tb.HasCheckConstraint("address_address_type_check", $"\"AddressType\" IN ({string.Join(",", Enum.GetValues<AddressType>().Select(e => $"'{e}'"))})"));
        }
    }
}