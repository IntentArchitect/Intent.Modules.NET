using EntityFrameworkCore.MySql.Domain.Entities.Accounts;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Infrastructure.Persistence.Configurations.Accounts
{
    public class AccTableConfiguration : IEntityTypeConfiguration<AccTable>
    {
        public void Configure(EntityTypeBuilder<AccTable> builder)
        {
            builder.ToTable("AccTables", "accounts");

            builder.HasKey(x => x.Id);
        }
    }
}