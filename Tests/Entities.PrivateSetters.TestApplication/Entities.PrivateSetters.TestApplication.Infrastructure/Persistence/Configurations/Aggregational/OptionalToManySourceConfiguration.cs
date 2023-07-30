using Entities.PrivateSetters.TestApplication.Domain.Entities.Aggregational;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Infrastructure.Persistence.Configurations.Aggregational
{
    public class OptionalToManySourceConfiguration : IEntityTypeConfiguration<OptionalToManySource>
    {
        public void Configure(EntityTypeBuilder<OptionalToManySource> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Attribute)
                .IsRequired();

            builder.HasMany(x => x.OptionalOneToManyDests)
                .WithOne(x => x.OptionalOneToManySource)
                .HasForeignKey(x => x.OptionalOneToManySourceId);
        }
    }
}