using EntityFrameworkCore.Postgres.Domain.Entities.NotSchema;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Infrastructure.Persistence.Configurations.NotSchema
{
    public class ViewFolderConfiguration : IEntityTypeConfiguration<ViewFolder>
    {
        public void Configure(EntityTypeBuilder<ViewFolder> builder)
        {
            builder.ToView("ViewFolders", "myapp");

            builder.HasKey(x => x.Id);
        }
    }
}