using AzureFunctions.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AzureFunctions.TestApplication.Infrastructure.Persistence.Configurations
{
    public class SampleDomainConfiguration : IEntityTypeConfiguration<SampleDomain>
    {
        public void Configure(EntityTypeBuilder<SampleDomain> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Attribute)
                .IsRequired();

            builder.Property(x => x.Name)
                .IsRequired();
        }
    }
}