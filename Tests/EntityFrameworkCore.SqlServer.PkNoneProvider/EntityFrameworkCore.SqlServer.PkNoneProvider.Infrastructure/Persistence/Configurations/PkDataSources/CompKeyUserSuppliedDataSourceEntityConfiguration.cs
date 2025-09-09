using EntityFrameworkCore.SqlServer.PkNoneProvider.Domain.Entities.PkDataSources;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.PkNoneProvider.Infrastructure.Persistence.Configurations.PkDataSources
{
    public class CompKeyUserSuppliedDataSourceEntityConfiguration : IEntityTypeConfiguration<CompKeyUserSuppliedDataSourceEntity>
    {
        public void Configure(EntityTypeBuilder<CompKeyUserSuppliedDataSourceEntity> builder)
        {
            builder.HasKey(x => new { x.Id1, x.Id2 });

            builder.Property(x => x.Id1)
                .ValueGeneratedNever();

            builder.Property(x => x.Id2)
                .ValueGeneratedNever();

            builder.Property(x => x.FieldValue)
                .IsRequired();
        }
    }
}