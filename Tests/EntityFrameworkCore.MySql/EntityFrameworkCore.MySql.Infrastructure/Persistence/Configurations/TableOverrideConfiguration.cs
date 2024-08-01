using EntityFrameworkCore.MySql.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Infrastructure.Persistence.Configurations
{
    public class TableOverrideConfiguration : IEntityTypeConfiguration<TableOverride>
    {
        public void Configure(EntityTypeBuilder<TableOverride> builder)
        {
            builder.ToTable("TableOverrides", "myschema");

            builder.HasKey(x => x.Id);
        }
    }
}