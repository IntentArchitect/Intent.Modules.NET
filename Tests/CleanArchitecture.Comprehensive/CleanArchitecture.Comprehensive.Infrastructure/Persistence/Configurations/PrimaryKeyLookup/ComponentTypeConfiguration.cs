using CleanArchitecture.Comprehensive.Domain.Entities.PrimaryKeyLookup;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Infrastructure.Persistence.Configurations.PrimaryKeyLookup
{
    public class ComponentTypeConfiguration : IEntityTypeConfiguration<ComponentType>
    {
        public void Configure(EntityTypeBuilder<ComponentType> builder)
        {
            builder.HasKey(x => x.ComponentTypeId);

            builder.Property(x => x.ComponentName)
                .IsRequired();

            builder.OwnsMany(x => x.ComponentPropertyGroups, ConfigureComponentPropertyGroups);

            builder.Ignore(e => e.DomainEvents);
        }

        public static void ConfigureComponentPropertyGroups(OwnedNavigationBuilder<ComponentType, ComponentPropertyGroup> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.ComponentTypeId);

            builder.HasKey(x => x.PropertyGroupId);

            builder.Property(x => x.GroupName)
                .IsRequired();

            builder.Property(x => x.ComponentTypeId)
                .IsRequired();

            builder.OwnsMany(x => x.ComponentTypeProperties, ConfigureComponentTypeProperties);
        }

        public static void ConfigureComponentTypeProperties(OwnedNavigationBuilder<ComponentPropertyGroup, ComponentTypeProperty> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.ComponentPropertyGroupPropertyGroupId);

            builder.HasKey(x => x.PropertyId);

            builder.Property(x => x.PropertyName)
                .IsRequired();

            builder.Property(x => x.ComponentPropertyGroupPropertyGroupId)
                .IsRequired();
        }
    }
}