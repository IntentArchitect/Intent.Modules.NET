using EntityFrameworkCore.SqlServer.EF8.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Infrastructure.Persistence.Configurations.Associations
{
    public class C_RequiredCompositeConfiguration : IEntityTypeConfiguration<C_RequiredComposite>
    {
        public void Configure(EntityTypeBuilder<C_RequiredComposite> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.RequiredCompAttr)
                .IsRequired();

            builder.OwnsMany(x => x.C_MultipleDependents, ConfigureC_MultipleDependents);
        }

        public void ConfigureC_MultipleDependents(OwnedNavigationBuilder<C_RequiredComposite, C_MultipleDependent> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.C_RequiredCompositeId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.MultipleDepAttr)
                .IsRequired();

            builder.Property(x => x.C_RequiredCompositeId)
                .IsRequired();
        }
    }
}