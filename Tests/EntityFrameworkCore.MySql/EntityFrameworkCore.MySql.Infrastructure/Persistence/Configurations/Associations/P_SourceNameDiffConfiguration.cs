using EntityFrameworkCore.MySql.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Infrastructure.Persistence.Configurations.Associations
{
    public class P_SourceNameDiffConfiguration : IEntityTypeConfiguration<P_SourceNameDiff>
    {
        public void Configure(EntityTypeBuilder<P_SourceNameDiff> builder)
        {
            builder.HasKey(x => x.Id);

            builder.OwnsMany(x => x.P_SourceNameDiffDependents, ConfigureP_SourceNameDiffDependents);
        }

        public void ConfigureP_SourceNameDiffDependents(OwnedNavigationBuilder<P_SourceNameDiff, P_SourceNameDiffDependent> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.SourceNameDiffId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.SourceNameDiffId)
                .IsRequired();
        }
    }
}