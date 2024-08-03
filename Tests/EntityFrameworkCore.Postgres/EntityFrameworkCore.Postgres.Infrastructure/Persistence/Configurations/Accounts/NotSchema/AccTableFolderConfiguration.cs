using EntityFrameworkCore.Postgres.Domain.Entities.Accounts.NotSchema;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Infrastructure.Persistence.Configurations.Accounts.NotSchema
{
    public class AccTableFolderConfiguration : IEntityTypeConfiguration<AccTableFolder>
    {
        public void Configure(EntityTypeBuilder<AccTableFolder> builder)
        {
            builder.ToTable("AccTableFolders", "accounts");

            builder.HasKey(x => x.Id);
        }
    }
}