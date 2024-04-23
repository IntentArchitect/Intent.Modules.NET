using EntityFrameworkCore.SqlServer.EF8.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Infrastructure.Persistence.Configurations.Associations
{
    public class G_RequiredCompositeNavConfiguration : IEntityTypeConfiguration<G_RequiredCompositeNav>
    {
        public void Configure(EntityTypeBuilder<G_RequiredCompositeNav> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ReqCompNavAttr)
                .IsRequired();

            builder.OwnsMany(x => x.G_MultipleDependents, ConfigureG_MultipleDependents);
        }

        public void ConfigureG_MultipleDependents(OwnedNavigationBuilder<G_RequiredCompositeNav, G_MultipleDependent> builder)
        {
            builder.WithOwner(x => x.G_RequiredCompositeNav)
                .HasForeignKey(x => x.G_RequiredCompositeNavId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.MultipleDepAttr)
                .IsRequired();

            builder.Property(x => x.G_RequiredCompositeNavId)
                .IsRequired();
        }
    }
}