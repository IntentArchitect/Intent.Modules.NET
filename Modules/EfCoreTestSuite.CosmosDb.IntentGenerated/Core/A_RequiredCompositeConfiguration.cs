using System;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Core
{
    public class A_RequiredCompositeConfiguration : IEntityTypeConfiguration<A_RequiredComposite>
    {
        public void Configure(EntityTypeBuilder<A_RequiredComposite> builder)
        {
            builder.ToTable("A_RequiredComposite");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.RequiredCompositeAttr)
                .IsRequired();

            builder.Property(x => x.PartitionKey)
                .IsRequired();
            builder.HasPartitionKey(x => x.PartitionKey);

            builder.OwnsOne(x => x.A_OptionalDependent, ConfigureA_OptionalDependent);

        }

        public void ConfigureA_OptionalDependent(OwnedNavigationBuilder<A_RequiredComposite, A_OptionalDependent> builder)
        {
            builder.WithOwner();
            builder.ToTable("A_OptionalDependent");


            builder.Property(x => x.OptionalDependentAttr)
                .IsRequired();
        }
    }
}