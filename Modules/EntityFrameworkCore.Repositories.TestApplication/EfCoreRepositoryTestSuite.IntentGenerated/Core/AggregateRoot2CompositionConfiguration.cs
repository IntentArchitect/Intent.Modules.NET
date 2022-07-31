using System;
using EfCoreRepositoryTestSuite.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreRepositoryTestSuite.IntentGenerated.Core
{
    public class AggregateRoot2CompositionConfiguration : IEntityTypeConfiguration<AggregateRoot2Composition>
    {
        public void Configure(EntityTypeBuilder<AggregateRoot2Composition> builder)
        {
            builder.HasKey(x => x.Id);


            builder.OwnsOne(x => x.AggregateRoot2Single, ConfigureAggregateRoot2Single);

            builder.OwnsOne(x => x.AggregateRoot2Nullable, ConfigureAggregateRoot2Nullable);

            builder.OwnsMany(x => x.AggregateRoot2Collections, ConfigureAggregateRoot2Collections);

        }

        public void ConfigureAggregateRoot2Single(OwnedNavigationBuilder<AggregateRoot2Composition, AggregateRoot2Single> builder)
        {
            builder.WithOwner().HasForeignKey(x => x.Id);
        }

        public void ConfigureAggregateRoot2Nullable(OwnedNavigationBuilder<AggregateRoot2Composition, AggregateRoot2Nullable> builder)
        {
            builder.WithOwner().HasForeignKey(x => x.Id);
        }

        public void ConfigureAggregateRoot2Collections(OwnedNavigationBuilder<AggregateRoot2Composition, AggregateRoot2Collection> builder)
        {
            builder.WithOwner().HasForeignKey(x => x.AggregateRoot2CompositionId);
        }
    }
}