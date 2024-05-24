using AdvancedMappingCrud.RichDomain.Tests.Domain;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Infrastructure.Persistence.Configurations
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.OwnsMany(x => x.ContactDetailsVOS, ConfigureContactDetailsVOS);

            builder.Ignore(e => e.DomainEvents);
        }

        public void ConfigureContactDetailsVOS(OwnedNavigationBuilder<Company, ContactDetailsVO> builder)
        {
            builder.WithOwner();

            builder.Property(x => x.Cell)
                .IsRequired();

            builder.Property(x => x.Email)
                .IsRequired();
        }
    }
}