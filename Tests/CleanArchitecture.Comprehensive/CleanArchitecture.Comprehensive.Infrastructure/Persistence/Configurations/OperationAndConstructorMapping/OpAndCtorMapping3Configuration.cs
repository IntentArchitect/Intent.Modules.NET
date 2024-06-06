using CleanArchitecture.Comprehensive.Domain.Entities.OperationAndConstructorMapping;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Infrastructure.Persistence.Configurations.OperationAndConstructorMapping
{
    public class OpAndCtorMapping3Configuration : IEntityTypeConfiguration<OpAndCtorMapping3>
    {
        public void Configure(EntityTypeBuilder<OpAndCtorMapping3> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}