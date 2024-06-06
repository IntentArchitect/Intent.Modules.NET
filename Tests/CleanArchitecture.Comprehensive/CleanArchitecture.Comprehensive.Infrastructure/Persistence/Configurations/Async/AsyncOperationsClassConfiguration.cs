using CleanArchitecture.Comprehensive.Domain.Entities.Async;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Infrastructure.Persistence.Configurations.Async
{
    public class AsyncOperationsClassConfiguration : IEntityTypeConfiguration<AsyncOperationsClass>
    {
        public void Configure(EntityTypeBuilder<AsyncOperationsClass> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}