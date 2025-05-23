using EntityFrameworkCore.SqlServer.EF8.Domain.Entities.PkDataSources;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Infrastructure.Persistence.Configurations.PkDataSources
{
    public class UserSuppliedDataSourceEntityConfiguration : IEntityTypeConfiguration<UserSuppliedDataSourceEntity>
    {
        public void Configure(EntityTypeBuilder<UserSuppliedDataSourceEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.FieldValue)
                .IsRequired();
        }
    }
}