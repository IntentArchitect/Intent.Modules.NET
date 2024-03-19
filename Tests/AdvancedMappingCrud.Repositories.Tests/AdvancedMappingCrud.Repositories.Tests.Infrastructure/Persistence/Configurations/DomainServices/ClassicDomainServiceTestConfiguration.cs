using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.DomainServices;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Infrastructure.Persistence.Configurations.DomainServices
{
    public class ClassicDomainServiceTestConfiguration : IEntityTypeConfiguration<ClassicDomainServiceTest>
    {
        public void Configure(EntityTypeBuilder<ClassicDomainServiceTest> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}