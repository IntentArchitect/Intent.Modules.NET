using EntityFrameworkCore.Postgres.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Infrastructure.Persistence.Configurations
{
    public class TableExplicitSchemaConfiguration : IEntityTypeConfiguration<TableExplicitSchema>
    {
        public void Configure(EntityTypeBuilder<TableExplicitSchema> builder)
        {
            builder.ToTable("TableExplicitSchemas", "explicit");

            builder.HasKey(x => x.Id);
        }
    }
}