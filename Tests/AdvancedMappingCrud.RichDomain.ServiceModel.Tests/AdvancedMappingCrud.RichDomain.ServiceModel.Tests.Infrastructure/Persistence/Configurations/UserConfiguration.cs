using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CompanyId)
                .IsRequired();

            builder.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.OwnsMany(x => x.Addresses, ConfigureAddresses);

            builder.OwnsOne(x => x.ContactDetails, ConfigureContactDetails)
                .Navigation(x => x.ContactDetails).IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }

        public static void ConfigureAddresses(OwnedNavigationBuilder<User, Address> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.UserId);

            builder.ToTable("Addresses");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.Line1)
                .IsRequired();

            builder.Property(x => x.Line2)
                .IsRequired();

            builder.Property(x => x.City)
                .IsRequired();

            builder.Property(x => x.Postal)
                .IsRequired();
        }

        public static void ConfigureContactDetails(OwnedNavigationBuilder<User, ContactDetailsVO> builder)
        {
            builder.WithOwner();

            builder.Property(x => x.Cell)
                .IsRequired();

            builder.Property(x => x.Email)
                .IsRequired();
        }
    }
}