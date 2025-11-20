using EntityFrameworkCore.SqlServer.EF10.Domain.Entities.ExplicitKeys;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF10.Infrastructure.Persistence.Configurations.ExplicitKeys
{
    public class PK_PrimaryKeyIntConfiguration : IEntityTypeConfiguration<PK_PrimaryKeyInt>
    {
        public void Configure(EntityTypeBuilder<PK_PrimaryKeyInt> builder)
        {
            builder.HasKey(x => x.PrimaryKeyId);
        }
    }
}