using IntegrationTesting.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace IntegrationTesting.Tests.Infrastructure.Persistence.Configurations
{
    public class UniqueConValConfiguration : IEntityTypeConfiguration<UniqueConVal>
    {
        public void Configure(EntityTypeBuilder<UniqueConVal> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Att1)
                .IsRequired();

            builder.Property(x => x.Att2)
                .IsRequired();

            builder.Property(x => x.AttInclude)
                .IsRequired();

            builder.HasIndex(x => new { x.Att1, x.Att2 })
                .IncludeProperties(x => new { x.AttInclude })
                .IsUnique();

            builder.Ignore(e => e.DomainEvents);
        }
    }
}