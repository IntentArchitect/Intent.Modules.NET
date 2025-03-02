using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Infrastructure.Persistence.Configurations
{
    public class MultiKeyParentConfiguration : IEntityTypeConfiguration<MultiKeyParent>
    {
        public void Configure(EntityTypeBuilder<MultiKeyParent> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.OwnsMany(x => x.MultiKeyChildren, ConfigureMultiKeyChildren);

            builder.Ignore(e => e.DomainEvents);
        }

        public static void ConfigureMultiKeyChildren(OwnedNavigationBuilder<MultiKeyParent, MultiKeyChild> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.MultiKeyParentId);

            builder.HasKey(x => new { x.Id, x.RefNo });

            builder.Property(x => x.ChildName)
                .IsRequired();

            builder.Property(x => x.MultiKeyParentId)
                .IsRequired();
        }
    }
}