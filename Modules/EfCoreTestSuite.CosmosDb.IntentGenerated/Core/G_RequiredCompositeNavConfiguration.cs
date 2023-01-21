using System;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Core
{
    public class G_RequiredCompositeNavConfiguration : IEntityTypeConfiguration<G_RequiredCompositeNav>
    {
        public void Configure(EntityTypeBuilder<G_RequiredCompositeNav> builder)
        {
            builder.ToContainer("PartitionKeyNamed");

            builder.HasPartitionKey(x => x.PartitionKey);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PartitionKey)
                .IsRequired();

            builder.Property(x => x.RequiredCompNavAttr)
                .IsRequired();

            builder.OwnsMany(x => x.GMultipleDependents, ConfigureGMultipleDependents);
        }

        public void ConfigureGMultipleDependents(OwnedNavigationBuilder<G_RequiredCompositeNav, G_MultipleDependent> builder)
        {
            builder.WithOwner(x => x.GRequiredCompositeNav);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.MultipleDepAttr)
                .IsRequired();
        }
    }
}