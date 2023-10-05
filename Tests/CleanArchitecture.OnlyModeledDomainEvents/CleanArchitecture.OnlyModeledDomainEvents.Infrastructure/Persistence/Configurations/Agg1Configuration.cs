using CleanArchitecture.OnlyModeledDomainEvents.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CleanArchitecture.OnlyModeledDomainEvents.Infrastructure.Persistence.Configurations
{
    public class Agg1Configuration : IEntityTypeConfiguration<Agg1>
    {
        public void Configure(EntityTypeBuilder<Agg1> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}