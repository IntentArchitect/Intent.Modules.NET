using EntityFrameworkCore.SqlServer.EF8.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Infrastructure.Persistence.Configurations.Associations
{
    public class R_SourceNameDiffConfiguration : IEntityTypeConfiguration<R_SourceNameDiff>
    {
        public void Configure(EntityTypeBuilder<R_SourceNameDiff> builder)
        {
            builder.HasKey(x => x.Id);

            builder.OwnsOne(x => x.R_SourceNameDiffDependent, ConfigureR_SourceNameDiffDependent)
                .Navigation(x => x.R_SourceNameDiffDependent).IsRequired();
        }

        public void ConfigureR_SourceNameDiffDependent(OwnedNavigationBuilder<R_SourceNameDiff, R_SourceNameDiffDependent> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.Id);

            builder.HasKey(x => x.Id);
        }
    }
}