using EntityFrameworkCore.Postgres.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Infrastructure.Persistence.Configurations
{
    public class TablePlainConfiguration : IEntityTypeConfiguration<TablePlain>
    {
        public void Configure(EntityTypeBuilder<TablePlain> builder)
        {
            builder.ToTable("TablePlains", "myapp");

            builder.HasKey(x => x.Id);
        }
    }
}