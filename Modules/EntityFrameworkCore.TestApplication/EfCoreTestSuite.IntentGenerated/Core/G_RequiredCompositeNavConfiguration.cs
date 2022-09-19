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
    public class G_RequiredCompositeNavConfiguration : IEntityTypeConfiguration<G_RequiredCompositeNav>
    {
        public void Configure(EntityTypeBuilder<G_RequiredCompositeNav> builder)
        {
            builder.HasKey(x => x.Id);

            builder.OwnsMany(x => x.G_MultipleDependents, ConfigureG_MultipleDependents);
        }

        public void ConfigureG_MultipleDependents(OwnedNavigationBuilder<G_RequiredCompositeNav, G_MultipleDependent> builder)
        {
            builder.WithOwner(x => x.G_RequiredCompositeNav).HasForeignKey(x => x.G_RequiredCompositeNavId);
        }
    }
}