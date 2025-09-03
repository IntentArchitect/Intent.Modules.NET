using EntityFrameworkCore.SqlServer.PkNoneProvider.Domain.Entities.ExplicitKeys;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.PkNoneProvider.Infrastructure.Persistence.Configurations.ExplicitKeys
{
    public class PK_A_CompositeKeyConfiguration : IEntityTypeConfiguration<PK_A_CompositeKey>
    {
        public void Configure(EntityTypeBuilder<PK_A_CompositeKey> builder)
        {
            builder.HasKey(x => new { x.CompositeKeyA, x.CompositeKeyB });

            builder.Property(x => x.CompositeKeyA)
                .ValueGeneratedNever();

            builder.Property(x => x.CompositeKeyB)
                .ValueGeneratedNever();
        }
    }
}