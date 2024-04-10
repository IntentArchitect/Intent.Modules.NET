using EntityFrameworkCore.SqlServer.EF8.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Infrastructure.Persistence.Configurations.Associations
{
    public class A_RequiredCompositeConfiguration : IEntityTypeConfiguration<A_RequiredComposite>
    {
        public void Configure(EntityTypeBuilder<A_RequiredComposite> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.RequiredCompAttr)
                .IsRequired();

            builder.OwnsOne(x => x.A_OptionalDependent, ConfigureA_OptionalDependent);
        }

        public void ConfigureA_OptionalDependent(OwnedNavigationBuilder<A_RequiredComposite, A_OptionalDependent> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.Id);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.OptionalDepAttr)
                .IsRequired();
        }
    }
}