using Entities.PrivateSetters.TestApplication.Domain.Entities.Aggregational;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Infrastructure.Persistence.Configurations.Aggregational
{
    public class OptionalToManyDestConfiguration : IEntityTypeConfiguration<OptionalToManyDest>
    {
        public void Configure(EntityTypeBuilder<OptionalToManyDest> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.OptionalOneToManySourceId);

            builder.Property(x => x.Attribute)
                .IsRequired();
        }
    }
}