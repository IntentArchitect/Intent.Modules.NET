using Entities.PrivateSetters.EF.CosmosDb.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace Entities.PrivateSetters.EF.CosmosDb.Infrastructure.Persistence.Configurations
{
    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.ToContainer("Entities.PrivateSetters.EF.CosmosDb");

            builder.HasPartitionKey(x => x.Id);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();
        }
    }
}