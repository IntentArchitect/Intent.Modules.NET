using EntityFrameworkCore.SqlServer.EF7.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Infrastructure.Persistence.Configurations.Associations
{
    public class Q_DestNameDiffConfiguration : IEntityTypeConfiguration<Q_DestNameDiff>
    {
        public void Configure(EntityTypeBuilder<Q_DestNameDiff> builder)
        {
            builder.HasKey(x => x.Id);

            builder.OwnsOne(x => x.DestNameDiff, ConfigureDestNameDiff)
                .Navigation(x => x.DestNameDiff).IsRequired();
        }

        public static void ConfigureDestNameDiff(OwnedNavigationBuilder<Q_DestNameDiff, Q_DestNameDiffDependent> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.Id);

            builder.HasKey(x => x.Id);
        }
    }
}