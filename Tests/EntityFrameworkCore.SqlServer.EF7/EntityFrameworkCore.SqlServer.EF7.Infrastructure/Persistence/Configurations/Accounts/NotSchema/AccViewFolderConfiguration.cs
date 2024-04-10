using EntityFrameworkCore.SqlServer.EF7.Domain.Entities.Accounts.NotSchema;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Infrastructure.Persistence.Configurations.Accounts.NotSchema
{
    public class AccViewFolderConfiguration : IEntityTypeConfiguration<AccViewFolder>
    {
        public void Configure(EntityTypeBuilder<AccViewFolder> builder)
        {
            builder.ToView("AccViewFolders", "accounts");

            builder.HasKey(x => x.Id);
        }
    }
}