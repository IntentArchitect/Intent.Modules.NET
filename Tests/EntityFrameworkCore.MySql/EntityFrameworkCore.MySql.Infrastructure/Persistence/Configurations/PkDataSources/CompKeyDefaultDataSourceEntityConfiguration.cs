using EntityFrameworkCore.MySql.Domain.Entities.PkDataSources;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Infrastructure.Persistence.Configurations.PkDataSources
{
    public class CompKeyDefaultDataSourceEntityConfiguration : IEntityTypeConfiguration<CompKeyDefaultDataSourceEntity>
    {
        public void Configure(EntityTypeBuilder<CompKeyDefaultDataSourceEntity> builder)
        {
            builder.HasKey(x => new { x.Id1, x.Id2 });

            builder.Property(x => x.FieldValue)
                .IsRequired();
        }
    }
}