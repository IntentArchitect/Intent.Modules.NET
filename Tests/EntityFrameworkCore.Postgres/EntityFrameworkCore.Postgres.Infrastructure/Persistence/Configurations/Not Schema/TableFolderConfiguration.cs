using EntityFrameworkCore.Postgres.Domain.Entities.NotSchema;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Infrastructure.Persistence.Configurations.NotSchema
{
    public class TableFolderConfiguration : IEntityTypeConfiguration<TableFolder>
    {
        public void Configure(EntityTypeBuilder<TableFolder> builder)
        {
            builder.ToTable("TableFolders", "myapp");

            builder.HasKey(x => x.Id);
        }
    }
}