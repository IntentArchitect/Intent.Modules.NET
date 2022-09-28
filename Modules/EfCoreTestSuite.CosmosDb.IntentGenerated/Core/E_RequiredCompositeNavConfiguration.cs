using System;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Core
{
    public class E_RequiredCompositeNavConfiguration : IEntityTypeConfiguration<E_RequiredCompositeNav>
    {
        public void Configure(EntityTypeBuilder<E_RequiredCompositeNav> builder)
        {
            builder.ToTable("E_RequiredCompositeNav");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.RequiredCompositeNavAttr)
                .IsRequired();

            builder.Property(x => x.PartitionKey)
                .IsRequired();
            builder.HasPartitionKey(x => x.PartitionKey);

            builder.OwnsOne(x => x.E_RequiredDependent, ConfigureE_RequiredDependent)
                .Navigation(x => x.E_RequiredDependent).IsRequired();
        }

        public void ConfigureE_RequiredDependent(OwnedNavigationBuilder<E_RequiredCompositeNav, E_RequiredDependent> builder)
        {
            builder.WithOwner(x => x.E_RequiredCompositeNav).HasForeignKey(x => x.Id);
            builder.ToTable("E_RequiredDependent");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.RequiredDependentAttr)
                .IsRequired();
        }
    }
}