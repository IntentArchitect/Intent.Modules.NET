using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.Folder;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Infrastructure.Persistence.Configurations.Folder
{
    public class FolderConfiguration : IEntityTypeConfiguration<Domain.Entities.Folder.Folder>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Folder.Folder> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Property(x => x.Code)
                .IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }
    }
}