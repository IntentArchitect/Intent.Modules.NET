using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.Indexes;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Persistence.Configurations.Indexes
{
    public class WithBaseIndexConfiguration : IEntityTypeConfiguration<WithBaseIndex>
    {
        public void Configure(EntityTypeBuilder<WithBaseIndex> builder)
        {
            builder.HasBaseType<WithBaseIndexBase>();

            builder.Property(x => x.Name)
                .IsRequired();
        }
    }
}