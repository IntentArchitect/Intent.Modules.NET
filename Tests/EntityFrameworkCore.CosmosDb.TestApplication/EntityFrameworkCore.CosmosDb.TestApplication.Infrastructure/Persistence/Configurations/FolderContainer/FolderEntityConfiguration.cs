using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.FolderContainer;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Infrastructure.Persistence.Configurations.FolderContainer
{
    public class FolderEntityConfiguration : IEntityTypeConfiguration<FolderEntity>
    {
        public void Configure(EntityTypeBuilder<FolderEntity> builder)
        {
            builder.ToContainer("FolderContainer");

            builder.HasPartitionKey(x => x.FolderPartKey);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.FolderPartKey)
                .IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }
    }
}