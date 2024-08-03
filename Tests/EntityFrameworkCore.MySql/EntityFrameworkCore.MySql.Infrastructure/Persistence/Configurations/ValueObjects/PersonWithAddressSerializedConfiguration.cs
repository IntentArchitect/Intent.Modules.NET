using EntityFrameworkCore.MySql.Domain.Entities.ValueObjects;
using EntityFrameworkCore.MySql.Domain.ValueObjects;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Infrastructure.Persistence.Configurations.ValueObjects
{
    public class PersonWithAddressSerializedConfiguration : IEntityTypeConfiguration<PersonWithAddressSerialized>
    {
        public void Configure(EntityTypeBuilder<PersonWithAddressSerialized> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.OwnsOne(x => x.AddressSerialized, ConfigureAddressSerialized)
                .Navigation(x => x.AddressSerialized).IsRequired();
        }

        public void ConfigureAddressSerialized(OwnedNavigationBuilder<PersonWithAddressSerialized, AddressSerialized> builder)
        {
            builder.WithOwner();

            builder.ToJson();

            builder.Property(x => x.Line1)
                .IsRequired();

            builder.Property(x => x.Line2)
                .IsRequired();

            builder.Property(x => x.City)
                .IsRequired();
        }
    }
}