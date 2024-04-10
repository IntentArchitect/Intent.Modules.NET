using EntityFrameworkCore.SqlServer.EF8.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Infrastructure.Persistence.Configurations.Associations
{
    public class E_RequiredCompositeNavConfiguration : IEntityTypeConfiguration<E_RequiredCompositeNav>
    {
        public void Configure(EntityTypeBuilder<E_RequiredCompositeNav> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.RequiredCompNavAttr)
                .IsRequired();

            builder.OwnsOne(x => x.E_RequiredDependent, ConfigureE_RequiredDependent)
                .Navigation(x => x.E_RequiredDependent).IsRequired();
        }

        public void ConfigureE_RequiredDependent(OwnedNavigationBuilder<E_RequiredCompositeNav, E_RequiredDependent> builder)
        {
            builder.WithOwner(x => x.E_RequiredCompositeNav)
                .HasForeignKey(x => x.Id);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.RequiredDepAttr)
                .IsRequired();
        }
    }
}