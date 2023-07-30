using Entities.PrivateSetters.TestApplication.Domain.Entities.Compositional;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Infrastructure.Persistence.Configurations.Compositional
{
    public class OneToManySourceConfiguration : IEntityTypeConfiguration<OneToManySource>
    {
        public void Configure(EntityTypeBuilder<OneToManySource> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Attribute)
                .IsRequired();

            builder.OwnsMany(x => x.Owneds, ConfigureOwneds);
        }

        public void ConfigureOwneds(OwnedNavigationBuilder<OneToManySource, OneToManyDest> builder)
        {
            builder.WithOwner(x => x.Owner)
                .HasForeignKey(x => x.OwnerId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.OwnerId)
                .IsRequired();

            builder.Property(x => x.Attribute)
                .IsRequired();
        }
    }
}