using CqrsAutoCrud.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Infrastructure.Persistence.Configurations
{
    public class ImplicitKeyAggrRootConfiguration : IEntityTypeConfiguration<ImplicitKeyAggrRoot>
    {
        public void Configure(EntityTypeBuilder<ImplicitKeyAggrRoot> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Attribute)
                .IsRequired();

            builder.OwnsMany(x => x.ImplicitKeyNestedCompositions, ConfigureImplicitKeyNestedCompositions);
        }

        public void ConfigureImplicitKeyNestedCompositions(OwnedNavigationBuilder<ImplicitKeyAggrRoot, ImplicitKeyNestedComposition> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.ImplicitKeyAggrRootId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Attribute)
                .IsRequired();
        }
    }
}