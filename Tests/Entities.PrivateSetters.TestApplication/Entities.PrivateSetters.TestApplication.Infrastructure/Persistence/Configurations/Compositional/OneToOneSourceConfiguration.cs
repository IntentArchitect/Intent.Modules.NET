using Entities.PrivateSetters.TestApplication.Domain.Entities.Compositional;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Infrastructure.Persistence.Configurations.Compositional
{
    public class OneToOneSourceConfiguration : IEntityTypeConfiguration<OneToOneSource>
    {
        public void Configure(EntityTypeBuilder<OneToOneSource> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Attribute)
                .IsRequired();

            builder.OwnsOne(x => x.OneToOneDest, ConfigureOneToOneDest)
                .Navigation(x => x.OneToOneDest).IsRequired();
        }

        public void ConfigureOneToOneDest(OwnedNavigationBuilder<OneToOneSource, OneToOneDest> builder)
        {
            builder.WithOwner(x => x.OneToOneSource)
                .HasForeignKey(x => x.Id);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Attribute)
                .IsRequired();
        }
    }
}