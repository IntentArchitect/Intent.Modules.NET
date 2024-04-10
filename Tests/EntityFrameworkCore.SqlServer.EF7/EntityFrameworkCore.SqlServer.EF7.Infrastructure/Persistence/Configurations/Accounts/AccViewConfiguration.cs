using EntityFrameworkCore.SqlServer.EF7.Domain.Entities.Accounts;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Infrastructure.Persistence.Configurations.Accounts
{
    public class AccViewConfiguration : IEntityTypeConfiguration<AccView>
    {
        public void Configure(EntityTypeBuilder<AccView> builder)
        {
            builder.ToView("AccViews", "accounts");

            builder.HasKey(x => x.Id);
        }
    }
}