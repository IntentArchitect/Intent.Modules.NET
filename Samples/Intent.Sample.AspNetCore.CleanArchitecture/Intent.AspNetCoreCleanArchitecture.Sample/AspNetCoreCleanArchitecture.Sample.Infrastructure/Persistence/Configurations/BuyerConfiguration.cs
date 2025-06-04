using AspNetCoreCleanArchitecture.Sample.Domain;
using AspNetCoreCleanArchitecture.Sample.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AspNetCoreCleanArchitecture.Sample.Infrastructure.Persistence.Configurations
{
    public class BuyerConfiguration : IEntityTypeConfiguration<Buyer>
    {
        public void Configure(EntityTypeBuilder<Buyer> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(x => x.Surname)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.OwnsOne(x => x.Address, ConfigureAddress)
                .Navigation(x => x.Address).IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }

        public static void ConfigureAddress(OwnedNavigationBuilder<Buyer, Address> builder)
        {
            builder.WithOwner();

            builder.Property(x => x.Line1)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Line2)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.City)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.PostalCode)
                .IsRequired()
                .HasMaxLength(20);
        }
    }
}