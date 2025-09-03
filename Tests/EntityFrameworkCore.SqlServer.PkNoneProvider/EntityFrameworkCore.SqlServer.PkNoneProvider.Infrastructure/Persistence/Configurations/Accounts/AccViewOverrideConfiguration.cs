using EntityFrameworkCore.SqlServer.PkNoneProvider.Domain.Entities.Accounts;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.PkNoneProvider.Infrastructure.Persistence.Configurations.Accounts
{
    public class AccViewOverrideConfiguration : IEntityTypeConfiguration<AccViewOverride>
    {
        public void Configure(EntityTypeBuilder<AccViewOverride> builder)
        {
            builder.ToView("AccViewOverrides", "imoutofcontrol");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();
        }
    }
}