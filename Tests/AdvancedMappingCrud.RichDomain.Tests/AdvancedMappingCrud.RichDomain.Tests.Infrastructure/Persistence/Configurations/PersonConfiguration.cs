using AdvancedMappingCrud.RichDomain.Tests.Domain;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Infrastructure.Persistence.Configurations
{
    public class PersonConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.HasKey(x => x.Id);

            builder.OwnsOne(x => x.Details, ConfigureDetails)
                .Navigation(x => x.Details).IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }

        public void ConfigureDetails(OwnedNavigationBuilder<Person, PersonDetails> builder)
        {
            builder.OwnsOne(x => x.Name, ConfigureName)
                .Navigation(x => x.Name).IsRequired();
        }

        public void ConfigureName(OwnedNavigationBuilder<PersonDetails, Names> builder)
        {
            builder.Property(x => x.First)
                .IsRequired();

            builder.Property(x => x.Last)
                .IsRequired();
        }
    }
}