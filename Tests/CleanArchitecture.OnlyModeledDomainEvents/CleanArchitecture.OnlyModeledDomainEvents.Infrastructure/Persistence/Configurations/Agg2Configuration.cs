using CleanArchitecture.OnlyModeledDomainEvents.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CleanArchitecture.OnlyModeledDomainEvents.Infrastructure.Persistence.Configurations
{
    public class Agg2Configuration : IEntityTypeConfiguration<Agg2>
    {
        public void Configure(EntityTypeBuilder<Agg2> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}