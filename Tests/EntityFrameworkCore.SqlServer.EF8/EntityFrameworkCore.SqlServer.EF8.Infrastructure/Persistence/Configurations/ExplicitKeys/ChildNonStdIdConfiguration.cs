using EntityFrameworkCore.SqlServer.EF8.Domain.Entities.ExplicitKeys;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Infrastructure.Persistence.Configurations.ExplicitKeys
{
    public class ChildNonStdIdConfiguration : IEntityTypeConfiguration<ChildNonStdId>
    {
        public void Configure(EntityTypeBuilder<ChildNonStdId> builder)
        {
            builder.HasKey(x => x.DiffId);

            builder.Property(x => x.Name)
                .IsRequired();
        }
    }
}