using EntityFrameworkCore.MySql.Domain.Entities.ExplicitKeys;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Infrastructure.Persistence.Configurations.ExplicitKeys
{
    public class PK_PrimaryKeyLongConfiguration : IEntityTypeConfiguration<PK_PrimaryKeyLong>
    {
        public void Configure(EntityTypeBuilder<PK_PrimaryKeyLong> builder)
        {
            builder.HasKey(x => x.PrimaryKeyLong);
        }
    }
}