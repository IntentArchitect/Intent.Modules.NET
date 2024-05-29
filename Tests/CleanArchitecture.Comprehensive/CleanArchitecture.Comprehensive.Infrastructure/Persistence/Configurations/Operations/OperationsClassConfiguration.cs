using CleanArchitecture.Comprehensive.Domain.Entities.Operations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Infrastructure.Persistence.Configurations.Operations
{
    public class OperationsClassConfiguration : IEntityTypeConfiguration<OperationsClass>
    {
        public void Configure(EntityTypeBuilder<OperationsClass> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}