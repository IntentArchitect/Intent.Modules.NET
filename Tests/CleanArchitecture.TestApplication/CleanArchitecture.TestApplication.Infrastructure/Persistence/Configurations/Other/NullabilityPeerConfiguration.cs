using CleanArchitecture.TestApplication.Domain.Entities.Other;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Infrastructure.Persistence.Configurations.Other
{
    public class NullabilityPeerConfiguration : IEntityTypeConfiguration<NullabilityPeer>
    {
        public void Configure(EntityTypeBuilder<NullabilityPeer> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}