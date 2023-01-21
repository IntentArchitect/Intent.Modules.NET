using System;
using EfCoreTestSuite.IntentGenerated.Entities;
using EfCoreTestSuite.IntentGenerated.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Core
{
    public class E_RequiredCompositeNavConfiguration : IEntityTypeConfiguration<E_RequiredCompositeNav>
    {
        public void Configure(EntityTypeBuilder<E_RequiredCompositeNav> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.RequiredCompNavAttr)
                .IsRequired();

            builder.OwnsOne(x => x.ERequiredDependent, ConfigureERequiredDependent)
                .Navigation(x => x.ERequiredDependent).IsRequired();
        }

        public void ConfigureERequiredDependent(OwnedNavigationBuilder<E_RequiredCompositeNav, E_RequiredDependent> builder)
        {
            builder.WithOwner(x => x.ERequiredCompositeNav)
                .HasForeignKey(x => x.Id);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.RequiredDepAttr)
                .IsRequired();
        }
    }
}