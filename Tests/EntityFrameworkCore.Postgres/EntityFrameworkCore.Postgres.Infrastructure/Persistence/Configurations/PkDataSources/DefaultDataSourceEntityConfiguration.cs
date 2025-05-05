using EntityFrameworkCore.Postgres.Domain.Entities.PkDataSources;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Infrastructure.Persistence.Configurations.PkDataSources
{
    public class DefaultDataSourceEntityConfiguration : IEntityTypeConfiguration<DefaultDataSourceEntity>
    {
        public void Configure(EntityTypeBuilder<DefaultDataSourceEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.FieldValue)
                .IsRequired();
        }
    }
}