using Entities.PrivateSetters.TestApplication.Domain.Entities.Mapping;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Infrastructure.Persistence.Configurations.Mapping
{
    public class MappingRootConfiguration : IEntityTypeConfiguration<MappingRoot>
    {
        public void Configure(EntityTypeBuilder<MappingRoot> builder)
        {
            builder.HasKey(x => x.Id);

            builder.OwnsMany(x => x.MappingCompositeMultiples, ConfigureMappingCompositeMultiples);

            builder.OwnsOne(x => x.MappingCompositeSingle, ConfigureMappingCompositeSingle)
                .Navigation(x => x.MappingCompositeSingle).IsRequired();
        }

        public void ConfigureMappingCompositeMultiples(OwnedNavigationBuilder<MappingRoot, MappingCompositeMultiple> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.MappingRootId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.MultipleValue)
                .IsRequired();

            builder.Property(x => x.MappingRootId)
                .IsRequired();
        }

        public void ConfigureMappingCompositeSingle(OwnedNavigationBuilder<MappingRoot, MappingCompositeSingle> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.Id);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.SingleValue)
                .IsRequired();
        }
    }
}