using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Infrastructure.Persistence.Configurations
{
    public class ContactSecondConfiguration : IEntityTypeConfiguration<ContactSecond>
    {
        public void Configure(EntityTypeBuilder<ContactSecond> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ContactName)
                .IsRequired();

            builder.OwnsOne(x => x.ContactDetailsSecond, ConfigureContactDetailsSecond)
                .Navigation(x => x.ContactDetailsSecond).IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }

        public static void ConfigureContactDetailsSecond(OwnedNavigationBuilder<ContactSecond, ContactDetailsSecond> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.Id);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();
        }
    }
}