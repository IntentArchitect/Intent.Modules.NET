using IntegrationTesting.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace IntegrationTesting.Tests.Infrastructure.Persistence.Configurations
{
    public class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.OwnsMany(x => x.States, ConfigureStates);

            builder.Ignore(e => e.DomainEvents);
        }

        public static void ConfigureStates(OwnedNavigationBuilder<Country, State> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.CountryId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Property(x => x.CountryId)
                .IsRequired();

            builder.OwnsMany(x => x.Cities, ConfigureCities);
        }

        public static void ConfigureCities(OwnedNavigationBuilder<State, City> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.StateId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Property(x => x.StateId)
                .IsRequired();
        }
    }
}