using EntityFrameworkCore.SqlServer.PkNoneProvider.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.PkNoneProvider.Infrastructure.Persistence.Configurations.Associations
{
    public class R_SourceNameDiffConfiguration : IEntityTypeConfiguration<R_SourceNameDiff>
    {
        public void Configure(EntityTypeBuilder<R_SourceNameDiff> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.OwnsOne(x => x.R_SourceNameDiffDependent, ConfigureR_SourceNameDiffDependent)
                .Navigation(x => x.R_SourceNameDiffDependent).IsRequired();
        }

        public static void ConfigureR_SourceNameDiffDependent(OwnedNavigationBuilder<R_SourceNameDiff, R_SourceNameDiffDependent> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.Id);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();
        }
    }
}