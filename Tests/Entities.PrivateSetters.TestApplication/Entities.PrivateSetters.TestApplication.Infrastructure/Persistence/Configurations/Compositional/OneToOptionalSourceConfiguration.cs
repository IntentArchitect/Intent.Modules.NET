using Entities.PrivateSetters.TestApplication.Domain.Entities.Compositional;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Infrastructure.Persistence.Configurations.Compositional
{
    public class OneToOptionalSourceConfiguration : IEntityTypeConfiguration<OneToOptionalSource>
    {
        public void Configure(EntityTypeBuilder<OneToOptionalSource> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Attribute)
                .IsRequired();

            builder.OwnsOne(x => x.OneToOptionalDest, ConfigureOneToOptionalDest);
        }

        public void ConfigureOneToOptionalDest(OwnedNavigationBuilder<OneToOptionalSource, OneToOptionalDest> builder)
        {
            builder.WithOwner(x => x.OneToOptionalSource)
                .HasForeignKey(x => x.Id);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Attribute)
                .IsRequired();
        }
    }
}