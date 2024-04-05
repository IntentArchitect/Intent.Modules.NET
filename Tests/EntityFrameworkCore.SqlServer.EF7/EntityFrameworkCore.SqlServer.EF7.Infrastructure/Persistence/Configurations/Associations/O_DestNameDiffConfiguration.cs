using EntityFrameworkCore.SqlServer.EF7.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Infrastructure.Persistence.Configurations.Associations
{
    public class O_DestNameDiffConfiguration : IEntityTypeConfiguration<O_DestNameDiff>
    {
        public void Configure(EntityTypeBuilder<O_DestNameDiff> builder)
        {
            builder.HasKey(x => x.Id);

            builder.OwnsMany(x => x.DestNameDiff, ConfigureDestNameDiff);
        }

        public void ConfigureDestNameDiff(OwnedNavigationBuilder<O_DestNameDiff, O_DestNameDiffDependent> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.ODestnamediffId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ODestnamediffId)
                .IsRequired();
        }
    }
}